using System;
using System.Linq;

using Assets.Zilon.Scripts.Services;

using UnityEngine;
using UnityEngine.Localization.Settings;

using Zenject;

public class ChangeLanguageButtonHandler : MonoBehaviour
{
    [Inject]
    private readonly UiSettingService _uiSettingService;

    public void ChangeLanguage()
    {
        var currentLanguage = _uiSettingService.CurrentLanguage;

        switch (currentLanguage)
        {
            case Language.English:
                _uiSettingService.CurrentLanguage = Language.Russian;
                SetLocaleByCode(SystemLanguage.Russian);
                break;

            case Language.Russian:
                _uiSettingService.CurrentLanguage = Language.English;
                SetLocaleByCode(SystemLanguage.English);
                break;

            case Language.Undefined:
            default:
                if (Debug.isDebugBuild)
                {
                    throw new ArgumentException($"Incorrect UI language value {currentLanguage}.");
                }

                _uiSettingService.CurrentLanguage = Language.English;
                SetLocaleByCode(SystemLanguage.English);
                break;
        }
    }

    private static void SetLocaleByCode(SystemLanguage targetLanguage)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(targetLanguage);
    }
}
