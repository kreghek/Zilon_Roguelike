using System;

using Assets.Zilon.Scripts;

using JetBrains.Annotations;

using UnityEngine;

using Zenject;

using Zilon.Core.Persons;
using Zilon.Core.Tactics;

public class PerksModalBody : MonoBehaviour, IModalWindowHandler
{

    private IActor _actor;

    public Transform PerkItemsParent;
    public PerkItemViewModel PerkItemPrefab;

    [NotNull] [Inject] private DiContainer _diContainer;

    public event EventHandler Closed;

    public string Caption => "Character";

    public void Start()
    {

    }

    public void Init(IActor actor)
    {
        _actor = actor;
        var evolutionData = _actor.Person.EvolutionData;
        UpdatePerksInner(PerkItemsParent, evolutionData.Perks);
    }

    private void UpdatePerksInner(Transform itemsParent, IPerk[] perks)
    {
        foreach (Transform itemTranform in itemsParent)
        {
            Destroy(itemTranform.gameObject);
        }

        foreach (var perk in perks)
        {
            var propItemVm = Instantiate(PerkItemPrefab, itemsParent);
            propItemVm.Init(perk);
        }
    }

    public void ApplyChanges()
    {

    }

    public void CancelChanges()
    {
        throw new System.NotImplementedException();
    }
}
