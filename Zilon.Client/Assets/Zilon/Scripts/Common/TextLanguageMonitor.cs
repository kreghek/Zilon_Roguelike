using System;

using Assets.Zilon.Scripts.Services;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

public class TextLanguageMonitor : MonoBehaviour
{
    [Inject]
    private readonly UiSettingService _uiSettingService;

    public Text TargetText;

    public string EnglishString;

    public string RussianString;

    public void Start()
    {
        InitDefaultTextValue();

        _uiSettingService.CurrentLanguageChanged += UiSettingService_CurrentLanguageChanged;
    }

    private void InitDefaultTextValue()
    {
        var currentLanguage = _uiSettingService.CurrentLanguage;

        SetTextValueByLanguage(currentLanguage);
    }

    private void SetTextValueByLanguage(Language currentLanguage)
    {
        switch (currentLanguage)
        {
            case Language.English:
                TargetText.text = EnglishString;
                break;

            case Language.Russian:
                TargetText.text = RussianString;
                break;

            case Language.Undefined:
            default:
                if (Debug.isDebugBuild || Application.isEditor)
                {
                    throw new ArgumentException($"Incorrect UI language value {currentLanguage}.");
                }

                TargetText.text = EnglishString;
                break;
        }
    }

    public void OnDestroy()
    {
        _uiSettingService.CurrentLanguageChanged -= UiSettingService_CurrentLanguageChanged;
    }

    private void UiSettingService_CurrentLanguageChanged(object sender, EventArgs e)
    {
        var currentLanguage = _uiSettingService.CurrentLanguage;

        SetTextValueByLanguage(currentLanguage);
    }
}
