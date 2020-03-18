using System;
using System.Linq;
using Assets.Zilon.Scripts;
using Assets.Zilon.Scripts.Services;

using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Zilon.Core.Persons;

public class PersonCreateModalBody : MonoBehaviour, IModalWindowHandler
{
    [Inject]
    private readonly UiSettingService _uiSettingService;

    public Text DescriptionText;

    public string Caption
    {
        get
        {
            var currentLanguage = _uiSettingService.CurrentLanguage;
            var caption = StaticPhrases.GetValue("caption-person-create", currentLanguage);

            return caption;
        }
    }

    public event EventHandler Closed;

    public void Init(HumanPerson playerPerson)
    {
        DescriptionText.text = string.Empty;

        var currentLanguage = _uiSettingService.CurrentLanguage;

        var buildInTraits = playerPerson.EvolutionData.Perks.Where(x => x.Scheme.IsBuildIn).ToArray();
        foreach (var startTrait in buildInTraits)
        {
            var traitName = LocalizationHelper.GetValueOrDefaultNoname(currentLanguage, startTrait.Scheme.Name);
            DescriptionText.text += traitName + "\n";
        }
    }

    public void ApplyChanges()
    {

    }

    public void CancelChanges()
    {
        throw new NotImplementedException();
    }
}
