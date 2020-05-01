using System.Linq;

using Assets.Zilon.Scripts.Services;

using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.PersonModules;
using Zilon.Core.Props;

public class NoBulletHandler : MonoBehaviour
{
    public GameObject Content;
    public Text MessageText;

    [Inject] [NotNull] private readonly ISectorUiState _playerState;
    [Inject] [NotNull] private readonly UiSettingService _uiSettingService;

    private void Start()
    {
        var currentLanguage = _uiSettingService.CurrentLanguage;
        var noBulletsDisplayText = StaticPhrases.GetValue("no-bullets", currentLanguage);
        MessageText.text = noBulletsDisplayText;
        InvokeRepeating(nameof(UpdateSprite), 0, 1);
    }

    private void UpdateSprite()
    {
        var person = _playerState.ActiveActor?.Actor.Person;
        if (person == null)
        {
            return;
        }

        var activeAct = _playerState.TacticalAct;
        if (activeAct is null)
        {
            return;
        }

        if (activeAct.Constrains?.PropResourceType != null)
        {
            var bulletInInventory = from resource in person.GetModule<IInventoryModule>().CalcActualItems().OfType<Resource>()
                                    where resource.Scheme.Bullet?.Caliber == activeAct.Constrains.PropResourceType
                                    where resource.Count >= activeAct.Constrains.PropResourceCount
                                    select resource;

            Content.SetActive(!bulletInInventory.Any());
        }
        else
        {
            Content.SetActive(false);
        }
    }
}
