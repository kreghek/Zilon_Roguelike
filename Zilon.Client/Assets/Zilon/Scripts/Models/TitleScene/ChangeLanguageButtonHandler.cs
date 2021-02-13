using System;

using Assets.Zilon.Scripts.Services;

using UnityEngine;

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
                break;

            case Language.Russian:
                _uiSettingService.CurrentLanguage = Language.English;
                break;

            case Language.Undefined:
            default:
                if (Debug.isDebugBuild)
                {
                    throw new ArgumentException($"Incorrect UI language value {currentLanguage}.");
                }

                _uiSettingService.CurrentLanguage = Language.English;
                break;
        }
    }
}
