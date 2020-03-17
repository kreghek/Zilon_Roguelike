using System;

using Assets.Zilon.Scripts.Models;
using Assets.Zilon.Scripts.Services;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

/// <summary>
/// Всплывающее окно с краткой информацией о перке.
/// </summary>
/// <remarks>
/// Для подробной информации будет использовать отдельный модал.
/// </remarks>
public class PerkInfoPopup : MonoBehaviour
{
    [Inject] private readonly UiSettingService _uiSettingService;

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
        var currentLanguage = _uiSettingService.CurrentLanguage;

        var perkScheme = perkViewModel.Perk.Scheme;

        NameText.text = LocalizationHelper.GetValueOrDefaultNoname(currentLanguage, perkScheme.Name);
        DescriptionText.text = LocalizationHelper.GetValue(currentLanguage, perkScheme.Description);

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

    public void Update()
    {
        if (PerkViewModel != null)
        {
            GetComponent<RectTransform>().position = PerkViewModel.Position
                + new Vector3(0.4f, -0.4f);
        }
    }
}