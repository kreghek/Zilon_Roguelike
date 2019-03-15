using System.Linq;

using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Props;

public class NoBulletHandler : MonoBehaviour
{
    public Image IconSprite;

    [Inject] [NotNull] private ISectorUiState _playerState;

    private void Start()
    {
        InvokeRepeating(nameof(UpdateSprite), 0, 1);
    }

    private void UpdateSprite()
    {
        var person = _playerState.ActiveActor?.Actor.Person;
        if (person == null)
        {
            return;
        }



        var activeAct = person.TacticalActCarrier.Acts.First();
        if (activeAct.Constrains != null)
        {
            var bulletInInventory = from resource in person.Inventory.CalcActualItems().OfType<Resource>()
                                    where resource.Scheme.Bullet?.Caliber == activeAct.Constrains.PropResourceType
                                    where resource.Count >= activeAct.Constrains.PropResourceCount
                                    select resource;

            IconSprite.enabled = !bulletInInventory.Any();
        }
        else
        {
            IconSprite.enabled = false;
        }
    }
}
