using System.Collections.Generic;
using System.Linq;

using Assets.Zilon.Scripts.Models;

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
    [Inject] private ISchemeService _schemeService;

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

        NameText.text = propScheme.Name?.En ?? propScheme.Name?.Ru ?? "[noname]";
        WritePropTags(prop);
        DescriptionText.text = propScheme.Description?.En ?? propScheme.Description?.Ru;

        WritePropStats(prop);
    }

    private void WritePropTags(IProp prop)
    {
        TagsText.text = null;

        if (prop.Scheme.Tags == null)
        {
            return;
        }

        var filteredTags = prop.Scheme.Tags.Where(x => !string.IsNullOrWhiteSpace(x));
        var tagsText = string.Join(" ", filteredTags);

        TagsText.text = tagsText;
    }

    private void WritePropStats(IProp prop)
    {
        StatText.text = null;
        var propScheme = prop.Scheme;

        switch (prop)
        {
            case Equipment equipment:

                var descriptionLines = new List<string>();

                if (propScheme.Equip.ActSids != null)
                {

                    var actDescriptions = new List<string>();
                    foreach (var sid in propScheme.Equip.ActSids)
                    {
                        var act = _schemeService.GetScheme<ITacticalActScheme>(sid);
                        var actName = act.Name.En ?? act.Name.Ru;
                        var efficient = GetEfficientString(act);
                        if (act.Stats.Effect == TacticalActEffectType.Damage)
                        {
                            var actImpact = act.Stats.Offence.Impact;
                            descriptionLines.Add($"{actName}: {actImpact} {efficient} efficient ({act.Stats.Offence.ApRank} rank)");
                        }
                        else if (act.Stats.Effect == TacticalActEffectType.Heal)
                        {
                            descriptionLines.Add($"{actName}: heal {efficient}");
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

                break;

            case Resource resource:
                if (resource.Scheme.Use != null)
                {
                    var ruleArray = resource.Scheme.Use.CommonRules.Select(GetUseRuleDescription);
                    var rules = string.Join("\n", ruleArray);
                    StatText.text = rules;
                }

                break;
        }
    }

    private static string GetEfficientString(ITacticalActScheme act)
    {
        var efficient = act.Stats.Efficient;
        var maxEfficientValue = efficient.Count * efficient.Dice;

        if (maxEfficientValue <= 5)
        {
            return "low";
        }
        else if (6 <= maxEfficientValue && maxEfficientValue <= 8)
        {
            return "normal";
        }
        else
        {
            return "high";
        }
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