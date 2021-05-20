using System;

using Assets.Zilon.Scripts.Services;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

public class TutorialPageHandler : MonoBehaviour
{
    [Inject]
    private readonly UiSettingService _uiSettingService;

    public Text DescriptionText;
    public Image DescriptionImage;

    public int PageIndex;

    public void Start()
    {
        var currentLanguage = _uiSettingService.CurrentLanguage;

        DescriptionText.text = GetLocalizedText(currentLanguage, $"page{PageIndex}");
    }

    private static string GetLocalizedText(Language currentLanguage, string mainKey)
    {
        string langKey;
        switch (currentLanguage)
        {
            case Language.Russian:
                langKey = "ru";
                break;

            case Language.English:
                langKey = "en";
                break;

            case Language.Undefined:
            default:
                langKey = GetDefaultOrThrowDebugException(currentLanguage);
                break;
        }

        var text = Resources.Load<TextAsset>($@"Tutorial\{mainKey}-{langKey}");

        return text.text;
    }

    private static string GetDefaultOrThrowDebugException(Language currentLanguage)
    {
        if (Debug.isDebugBuild)
        {
            throw new ArgumentException($"Incorrect UI language value {currentLanguage}.");
        }

        return "en";
    }
}
