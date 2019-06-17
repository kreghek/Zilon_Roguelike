using System;

using Assets.Zilon.Scripts.Models;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

using Zilon.Core.Components;
using Zilon.Core.Schemes;

/// <summary>
/// Всплывающее окно с краткой информацией о перке.
/// </summary>
/// <remarks>
/// Для подробной информации будет использовать отдельный модал.
/// </remarks>
public class PerkInfoPopup : MonoBehaviour
{
    [Inject] private ISchemeService _schemeService;

    public Text NameText;
    public Text DescriptionText;
    public Text JobsText;

    public IPerkViewModelDescription PerkViewModel { get; set; }

    public void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetPropViewModel(IPerkViewModelDescription perkViewModel)
    {
        PerkViewModel = perkViewModel;

        if (perkViewModel?.Perk != null)
        {
            gameObject.SetActive(true);
            ShowPerkInfo(perkViewModel);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void ShowPerkInfo(IPerkViewModelDescription perkViewModel)
    {
        var perkScheme = perkViewModel.Perk.Scheme;

        NameText.text = perkScheme.Name?.En ?? perkScheme.Name?.Ru ?? "[No Name]";
        DescriptionText.text = perkScheme.Description?.En ?? perkScheme.Description?.Ru;

        JobsText.text = null;

        if (perkViewModel.Perk.CurrentJobs != null)
        {
            foreach (var job in perkViewModel.Perk.CurrentJobs)
            {
                JobsText.text = $"{job.Scheme.Type}:{job.Progress}/{job.Scheme.Value}";

                if (job.IsComplete)
                {
                    JobsText.text = "[COMPLETE] " + JobsText.text;
                }

                JobsText.text += Environment.NewLine;
            }
        }
    }

    public void FixedUpdate()
    {
        if (PerkViewModel != null)
        {

            GetComponent<RectTransform>().position = PerkViewModel.Position
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
        return direction == PersonRuleDirection.Positive ? string.Empty : "-";
    }
}