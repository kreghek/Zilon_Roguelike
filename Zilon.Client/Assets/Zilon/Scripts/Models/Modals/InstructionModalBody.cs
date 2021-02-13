using System;

using Assets.Zilon.Scripts;
using Assets.Zilon.Scripts.Services;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

public class InstructionModalBody : MonoBehaviour, IModalWindowHandler
{
    [Inject]
    private readonly UiSettingService _uiSettingService;

    private int _pageCounter;

    public int PageCount;

    public Button NextButton;
    public Button PrevButton;

    public Text DescriptionText;
    public Image DescriptionImage;
    public Text PagesProgressText;

    public event EventHandler Closed;

    public string Caption => "Read The Fucking Manual!"; // Прочти Это Грёбанное Руководство.

    public CloseBehaviourOperation CloseBehaviour => CloseBehaviourOperation.DoNothing;

    public void Start()
    {
        _pageCounter = 0;
        ShowPage(_pageCounter);
    }

    public void NextButton_Handler()
    {
        _pageCounter++;

        if (_pageCounter >= PageCount)
        {
            _pageCounter = PageCount - 1;
        }

        var isFirstPath = _pageCounter == 0;
        PrevButton.gameObject.SetActive(!isFirstPath);

        var isLastPage = _pageCounter == PageCount - 1;
        NextButton.gameObject.SetActive(!isLastPage);

        ShowPage(_pageCounter);
    }

    public void PrevButton_Handler()
    {
        _pageCounter--;

        if (_pageCounter < 0)
        {
            _pageCounter = 0;
        }

        var isFirstPath = _pageCounter == 0;
        PrevButton.gameObject.SetActive(!isFirstPath);

        var isLastPage = _pageCounter == PageCount - 1;
        NextButton.gameObject.SetActive(!isLastPage);

        ShowPage(_pageCounter);
    }

    public void ApplyChanges()
    {

    }

    public void CancelChanges()
    {
        throw new NotImplementedException();
    }

    private void ShowPage(int pageIndex)
    {
        PagesProgressText.text = $"{pageIndex + 1}/{PageCount}";

        var currentLanguage = _uiSettingService.CurrentLanguage;

        DescriptionImage.sprite = GetManualImage($"page{pageIndex + 1}");
        DescriptionText.text = GetLocalizedManualText(currentLanguage, $"page{pageIndex + 1}");
    }

    private Sprite GetManualImage(string mainKey)
    {
        var sprite = Resources.Load<Sprite>($@"Tutorial\{mainKey}");
        return sprite;
    }

    private static string GetLocalizedManualText(Language currentLanguage, string mainKey)
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

            default:
            case Language.Undefined:
                if (Debug.isDebugBuild || Application.isEditor)
                {
                    throw new ArgumentException($"Некоректное значение языка {currentLanguage}.");
                }

                langKey = "en";

                break;
        }

        var text = Resources.Load<TextAsset>($@"Tutorial\{mainKey}-{langKey}");

        return text.text;
    }
}
