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
                SetLocaleByCode("ru");
                break;

            case Language.Russian:
                _uiSettingService.CurrentLanguage = Language.English;
                SetLocaleByCode("en");
                break;

            case Language.Undefined:
            default:
                if (Debug.isDebugBuild)
                {
                    throw new ArgumentException($"Incorrect UI language value {currentLanguage}.");
                }

                _uiSettingService.CurrentLanguage = Language.English;
                SetLocaleByCode("en");
                break;
        }
    }

    private static void SetLocaleByCode(string targetLocaleCode)
    {
        var locales = LocalizationSettings.AvailableLocales.Locales;
        var targetLocale = locales.SingleOrDefault(x => AreCodesEquals(targetLocaleCode, x.Identifier.Code));
        LocalizationSettings.SelectedLocale = targetLocale;
    }

    private static bool AreCodesEquals(string targetLocaleCode, string currentLocaleCode)
    {
        return string.Equals(
            currentLocaleCode,
            targetLocaleCode,
            StringComparison.InvariantCultureIgnoreCase);
    }
}
