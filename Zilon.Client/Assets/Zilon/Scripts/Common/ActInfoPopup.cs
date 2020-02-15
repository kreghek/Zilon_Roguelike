using System.Collections.Generic;
using System.Linq;

using Assets.Zilon.Scripts.Models;
using Assets.Zilon.Scripts.Services;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

using Zilon.Core.Components;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;

//TODO Попробовать объедининть с PerkInfoPopup
/// <summary>
/// Всплывающее окно с краткой информацией о предмете.
/// </summary>
/// <remarks>
/// Для подробной информации будет использовать отдельный модал.
/// </remarks>
public class ActInfoPopup : MonoBehaviour
{
    [Inject] private readonly ISchemeService _schemeService;
    [Inject] private readonly UiSettingService _uiSettingService;

    public Text NameText;
    public Text TagsText;
    public Text DescriptionText;
    public Text StatText;

    public IActViewModelDescription ActViewModel { get; set; }

    public void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetPropViewModel(IActViewModelDescription viewModel)
    {
        ActViewModel = viewModel;

        if (viewModel?.Act != null)
        {
            gameObject.SetActive(true);
            ShowPropInfo(viewModel);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void ShowPropInfo(IActViewModelDescription viewModel)
    {
        var act = viewModel.Act;

        NameText.text = GetPropDisplayName(act);
    }

    private string GetPropDescription(IPropScheme propScheme)
    {
        var lang = _uiSettingService.CurrentLanguage;
        var description = propScheme.Description;

        return LocalizationHelper.GetValue(lang, description);
    }

    private string GetPropDisplayName(ITacticalAct propScheme)
    {
        var lang = _uiSettingService.CurrentLanguage;
        var name = propScheme.Scheme.Name;

        return LocalizationHelper.GetValueOrDefaultNoname(lang, name);
    }

    public void Update()
    {
        if (ActViewModel != null)
        {

            GetComponent<RectTransform>().position = ActViewModel.Position
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