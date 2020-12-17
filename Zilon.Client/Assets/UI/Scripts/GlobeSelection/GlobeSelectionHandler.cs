using System;
using System.Linq;

using Assets.Zilon.Scripts.Services;

using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Zenject;

using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.Tactics;
using Zilon.Core.World;

public class GlobeSelectionHandler : MonoBehaviour
{
    [NotNull]
    [Inject]
    private readonly IGlobeInitializer _globeInitializer;

    [NotNull]
    [Inject]
    private readonly NationalUnityEventService _nationalUnityEventService;

    [NotNull]
    [Inject]
    private readonly UiSettingService _uiSettingService;

    [NotNull]
    [Inject]
    private readonly IPlayer _player;

    public GameObject NoGlobeParentObject;

    public GameObject GlobeDescriptionParentObject;

    public Text DescriptionText;

    public void Start()
    {
        NoGlobeParentObject.SetActive(true);
        GlobeDescriptionParentObject.SetActive(false);
    }

    public void GoButtonHandler()
    {
        SceneManager.LoadScene("combat");
    }

    public void GenerateGlobeButtonHandler()
    {
        var globe = _globeInitializer.CreateGlobeAsync("intro").Result;
        _nationalUnityEventService.Globe = globe;

        ShowPersonInfoInDescriptionTextbox(_player.MainPerson, DescriptionText, _uiSettingService);
        
        NoGlobeParentObject.SetActive(false);
        GlobeDescriptionParentObject.SetActive(true);
    }

    private static void ShowPersonInfoInDescriptionTextbox(IPerson playerPerson, Text DescriptionText, UiSettingService _uiSettingService)
    {
        var currentLanguage = _uiSettingService.CurrentLanguage;

        var backstoryText = GetLocalizedBackstoryText(currentLanguage, "main");
        DescriptionText.text = backstoryText + Environment.NewLine + Environment.NewLine;

        var className = LocalizationHelper.GetValueOrDefaultNoname(currentLanguage, (playerPerson as HumanPerson).PersonEquipmentTemplate);
        var classDescriptonText = GetLocalizedBackstoryText(currentLanguage, "class") + " " + className;
        DescriptionText.text += classDescriptonText + Environment.NewLine + Environment.NewLine;

        DescriptionText.text += GetLocalizedBackstoryText(currentLanguage, "trait") + Environment.NewLine;

        var buildInTraits = playerPerson.GetModule<IEvolutionModule>().Perks.Where(x => x.Scheme.IsBuildIn).ToArray();
        foreach (var startTrait in buildInTraits)
        {
            var traitName = LocalizationHelper.GetValueOrDefaultNoname(currentLanguage, startTrait.Scheme.Name);
            DescriptionText.text += " - " + traitName + Environment.NewLine;
        }

        DescriptionText.text += Environment.NewLine + Environment.NewLine;

        DescriptionText.text += GetLocalizedBackstoryText(currentLanguage, "props") + Environment.NewLine;
        foreach (var prop in playerPerson.GetModule<IEquipmentModule>())
        {
            if (prop is null)
            {
                continue;
            }

            var propName = LocalizationHelper.GetValueOrDefaultNoname(currentLanguage, prop.Scheme.Name);
            DescriptionText.text += " - " + propName + Environment.NewLine;
        }

        foreach (var prop in playerPerson.GetModule<IInventoryModule>().CalcActualItems())
        {
            var propName = LocalizationHelper.GetValueOrDefaultNoname(currentLanguage, prop.Scheme.Name);

            if (prop is Resource resource)
            {
                propName += " x " + resource.Count;
            }

            DescriptionText.text += " - " + propName + Environment.NewLine;
        }
    }

    private static string GetLocalizedBackstoryText(Language currentLanguage, string mainKey)
    {
        string langKey;
        switch (currentLanguage)
        {
            case Language.Russian:
                langKey = "ru";
                break;

            default:
            case Language.English:
                langKey = "en";
                break;

            case Language.Undefined:
                throw new ArgumentException($"Некоректное значение языка {currentLanguage}.");
        }
        var text = Resources.Load<TextAsset>($@"Backstory\{mainKey}-{langKey}");

        return text.text;
    }
}
