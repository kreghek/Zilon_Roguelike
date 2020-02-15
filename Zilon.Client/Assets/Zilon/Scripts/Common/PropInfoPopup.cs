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

        return LocalizationHelper.GetValueOrDefaultNoname(lang, name);
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

                    var apRankDisplayName = StaticPhrases.GetValue("ap-rank", currentLanguage);

                    descriptionLines.Add($"{actName}: {impactDisplayName} {efficient} ({act.Stats.Offence.ApRank} {apRankDisplayName})");
                }
                else if (act.Stats.Effect == TacticalActEffectType.Heal)
                {
                    var healDisplayName = StaticPhrases.GetValue("efficient-heal", currentLanguage);
                    descriptionLines.Add($"{actName}: {healDisplayName} {efficient}");
                }
            }
        }

        if (propScheme.Equip.Armors != null)
        {
            var protectsDisplayName = StaticPhrases.GetValue("armor-protects", currentLanguage);
            var armorRankDisplayName = StaticPhrases.GetValue("armor-rank", currentLanguage);
            foreach (var armor in propScheme.Equip.Armors)
            {
                var armorImpact = armor.Impact;
                var armorImpactLangKey = armorImpact.ToString().ToLowerInvariant();
                var impactDisplayName = StaticPhrases.GetValue($"impact-{armorImpactLangKey}", currentLanguage);

                var armorAbsorbtionLevel = armor.AbsorbtionLevel;
                var armorAbsorbtionLevelKey = armorAbsorbtionLevel.ToString().ToLowerInvariant();
                var armorAbsDisplayName = StaticPhrases.GetValue($"rule-{armorAbsorbtionLevelKey}", currentLanguage);

                descriptionLines.Add($"{protectsDisplayName}: {impactDisplayName} ({armor.ArmorRank} {armorRankDisplayName}): {armorAbsDisplayName}");
            }
        }

        if (propScheme.Equip.Rules != null)
        {
            foreach (var rule in propScheme.Equip.Rules)
            {
                var sign = GetDirectionString(rule.Direction);
                var bonusString = GetBonusString(rule.Direction);

                var ruleType = rule.Type;
                var ruleKey = ruleType.ToString().ToLowerInvariant();
                var ruleDisplayName = StaticPhrases.GetValue($"rule-{ruleKey}", currentLanguage);

                var ruleLevel = rule.Level;
                var ruleLevelKey = ruleLevel.ToString().ToLowerInvariant();
                var ruleLevelDisplayName = StaticPhrases.GetValue($"rule-{ruleLevelKey}", currentLanguage);

                descriptionLines.Add($"{bonusString}: {ruleDisplayName}: {sign}{ruleLevelDisplayName}");
            }
        }

        var durableDisplayName = StaticPhrases.GetValue($"prop-durable", currentLanguage);
        descriptionLines.Add($"{durableDisplayName}: {equipment.Durable.Value}/{equipment.Durable.Range.Max}");

        StatText.text = string.Join("\n", descriptionLines);
    }

    private static string GetEfficientString(ITacticalActScheme act)
    {
        return $"{act.Stats.Efficient.Count}D{act.Stats.Efficient.Dice}";
    }

    public void Update()
    {
        if (PropViewModel != null)
        {

            GetComponent<RectTransform>().position = PropViewModel.Position
                + new Vector3(0.4f, -0.4f);
        }
    }

    private string GetUseRuleDescription(ConsumeCommonRule rule)
    {
        var signString = GetDirectionString(rule.Direction);
        var currentLanguage = _uiSettingService.CurrentLanguage;

        var ruleType = rule.Type;
        var ruleKey = ruleType.ToString().ToLowerInvariant();
        var ruleDisplayName = StaticPhrases.GetValue($"rule-{ruleKey}", currentLanguage);

        var ruleLevel = rule.Level;
        var ruleLevelKey = ruleLevel.ToString().ToLowerInvariant();
        var ruleLevelDisplayName = StaticPhrases.GetValue($"rule-{ruleLevelKey}", currentLanguage);

        return $"{ruleDisplayName}: {signString}{ruleLevelDisplayName}";
    }

    private static string GetDirectionString(PersonRuleDirection direction)
    {
        if (direction == PersonRuleDirection.Negative)
        {
            return "-";
        }

        return string.Empty;
    }

    private string GetBonusString(PersonRuleDirection direction)
    {
        var currentLang = _uiSettingService.CurrentLanguage;
        var bonusDisplayName = StaticPhrases.GetValue("prop-bonus", currentLang);
        var penaltyDisplayName = StaticPhrases.GetValue("prop-penalty", currentLang);
        if (direction == PersonRuleDirection.Negative)
        {
            return penaltyDisplayName;
        }

        return bonusDisplayName;
    }
}