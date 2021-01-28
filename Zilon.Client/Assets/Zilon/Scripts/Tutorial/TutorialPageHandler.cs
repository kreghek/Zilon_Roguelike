using System;
using System.Collections;
using System.Collections.Generic;

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
            case Language.Undefined:
                if (Debug.isDebugBuild)
                {
                    throw new ArgumentException($"Некоректное значение языка {currentLanguage}.");
                }
                else
                {
                    langKey = "en";
                }
                break;

            case Language.Russian:
                langKey = "ru";
                break;


            case Language.English:
                langKey = "en";
                break;

            default:
                langKey = "en";
                break;
        }
        var text = Resources.Load<TextAsset>($@"Tutorial\{mainKey}-{langKey}");

        return text.text;
    }
}
