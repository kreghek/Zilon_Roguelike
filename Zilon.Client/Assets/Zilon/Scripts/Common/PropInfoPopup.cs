using System;
using System.Collections.Generic;
using System.Linq;

using Assets.Zilon.Scripts.Models;
using Assets.Zilon.Scripts.Services;
using UnityEngine;
using UnityEngine.UI;

using Zenject;

using Zilon.Core.Components;
using Zilon.Core.Props;
using Zilon.Core.Schemes;

//TODO Попробовать объедининть с PerkInfoPopup
/// <summary>
/// Всплывающее окно с краткой информацией о предмете.
/// </summary>
/// <remarks>
/// Для подробной информации будет использовать отдельный модал.
/// </remarks>
public class PropInfoPopup : MonoBehaviour
{
    [Inject] private readonly ISchemeService _schemeService;
    [Inject] private readonly UiSettingService _uiSettingService;

    public Text NameText;
    public Text TagsText;
    public Text DescriptionText;
    public Text StatText;

    public IPropViewModelDescription PropViewModel { get; set; }

    public void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetPropViewModel(IPropViewModelDescription propViewModel)
    {
        PropViewModel = propViewModel;

        if (propViewModel?.Prop != null)
        {
            gameObject.SetActive(true);
            ShowPropInfo(propViewModel);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void ShowPropInfo(IPropViewModelDescription propViewModel)
    {
        var prop = propViewModel.Prop;
        var propScheme = prop.Scheme;

        // обрабатываем лже-предметы
        propScheme = ProcessMimics(prop, propScheme);

        NameText.text = GetPropDisplayName(propScheme);
        WritePropTags(prop);
        DescriptionText.text = GetPropDescription(propScheme);

        WritePropStats(prop);
    }

    private string GetPropDescription(IPropScheme propScheme)
    {
        var lang = _uiSettingService.CurrentLanguage;
        var description = propScheme.Description;

        return LocalizationHelper.GetValue(lang, description);
    }

    private string GetPropDisplayName(IPropScheme propScheme)
    {
        var lang = _uiSettingService.CurrentLanguage;
        var name = propScheme.Name;

        return LocalizationHelper.GetValue(lang, name);
    }

    /// <summary>
    /// Лже-предметы выдают себя на нормальный указанный предмет.
    /// Но они не могут скрываться, когда их прочность падает ниже 50%.
    /// В рамках этого метода мы возвращаем либо реальную схему предмета,
    /// либо схему, под которую мимикрирует текущий предмет (потому что он мимик и его статы ниже).
    /// </summary>
    private IPropScheme ProcessMimics(IProp prop, IPropScheme propScheme)
    {
        if (propScheme.IsMimicFor != null)
        {
            if (prop is Equipment equipment)
            {
                if (equipment.Durable.ValueShare >= 0.5f)
                {
                    var mimicScheme = _schemeService.GetScheme<IPropScheme>(propScheme.IsMimicFor);
                    Debug.Assert(mimicScheme != null, "Все схемы должны быть согласованы");

                    propScheme = mimicScheme;
                }
            }
        }

        return propScheme;
    }

    private void WritePropTags(IProp prop)
    {
        TagsText.text = null;

        if (prop.Scheme.Tags == null)
        {
            return;
        }

        var filteredTags = prop.Scheme.Tags.Where(x => !string.IsNullOrWhiteSpace(x));
        var currentLanguage = _uiSettingService.CurrentLanguage;
        var tagDisplayNames = filteredTags.Select(x => StaticPhrases.GetValue($"prop-tag-{x}", currentLanguage));
        var tagsText = string.Join(" ", tagDisplayNames);

        TagsText.text = tagsText;
    }

    private void WritePropStats(IProp prop)
    {
        StatText.text = null;
        var propScheme = prop.Scheme;
        propScheme = ProcessMimics(prop, propScheme);

        switch (prop)
        {
            case Equipment equipment:
                WriteEquipmentStats(propScheme, equipment);
                break;

            case Resource resource:
                WriteResourceStats(resource);
                break;
        }
    }

    private void WriteResourceStats(Resource resource)
    {
        if (resource.Scheme.Use != null)
        {
            var ruleArray = resource.Scheme.Use.CommonRules.Select(GetUseRuleDescription);
            var rules = string.Join("\n", ruleArray);
            StatText.text = rules;
        }
    }

    private void WriteEquipmentStats(IPropScheme propScheme, Equipment equipment)
    {
        var currentLanguage = _uiSettingService.CurrentLanguage;
        var descriptionLines = new List<string>();

        if (propScheme.Equip.ActSids != null)
        {
            foreach (var sid in propScheme.Equip.ActSids)
            {
                var act = _schemeService.GetScheme<ITacticalActScheme>(sid);
                var actName = LocalizationHelper.GetValue(currentLanguage, act.Name);
                var efficient = GetEfficientString(act);
                if (act.Stats.Effect == TacticalActEffectType.Damage)
                {
                    var actImpact = act.Stats.Offence.Impact;
                    var actImpactLangKey = actImpact.ToString().ToLowerInvariant();
                    var impactDisplayName = StaticPhrases.GetValue($"impact-{actImpactLangKey}", currentLanguage);

                    var apRankDisplayName = StaticPhrases.GetValue($"ap-rank", currentLanguage);

                    descriptionLines.Add($"{actName}: {impactDisplayName} {efficient} ({act.Stats.Offence.ApRank} {apRankDisplayName})");
                }
                else if (act.Stats.Effect == TacticalActEffectType.Heal)
                {
                    var healDisplayName = StaticPhrases.GetValue($"efficient-heal", currentLanguage);
                    descriptionLines.Add($"{actName}: {healDisplayName} {efficient}");
                }
            }
        }

        if (propScheme.Equip.Armors != null)
        {
            foreach (var armor in propScheme.Equip.Armors)
            {
                descriptionLines.Add($"Protects: {armor.Impact} ({armor.ArmorRank} rank): {armor.AbsorbtionLevel}");
            }
        }

        if (propScheme.Equip.Rules != null)
        {
            foreach (var rule in propScheme.Equip.Rules)
            {
                var sign = GetDirectionString(rule.Direction);
                var bonusString = GetBonusString(rule.Direction);
                descriptionLines.Add($"{bonusString}: {rule.Type}: {sign}{rule.Level}");
            }
        }

        descriptionLines.Add($"Durable: {equipment.Durable.Value}/{equipment.Durable.Range.Max}");

        StatText.text = string.Join("\n", descriptionLines);
    }

    private static string GetEfficientString(ITacticalActScheme act)
    {
        return $"{act.Stats.Efficient.Count}D{act.Stats.Efficient.Dice}";
    }

    public void FixedUpdate()
    {
        if (PropViewModel != null)
        {

            GetComponent<RectTransform>().position = PropViewModel.Position
                + new Vector3(0.4f, -0.4f);
        }
    }

    private static string GetUseRuleDescription(ConsumeCommonRule rule)
    {
        string signString = GetDirectionString(rule.Direction);
        return $"{rule.Type}:{signString}{rule.Level}";
    }

    private static string GetDirectionString(PersonRuleDirection direction)
    {
        if (direction == PersonRuleDirection.Negative)
        {
            return "-";
        }

        return string.Empty;
    }

    private static string GetBonusString(PersonRuleDirection direction)
    {
        if (direction == PersonRuleDirection.Negative)
        {
            return "Penalty";
        }

        return "Bonus";
    }
}