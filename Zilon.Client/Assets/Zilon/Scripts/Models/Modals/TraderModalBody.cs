using System;
using System.Collections.Generic;

using Assets.Zilon.Scripts;

using JetBrains.Annotations;

using UnityEngine;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Props;
using Zilon.Core.Tactics;

public class TraderModalBody : MonoBehaviour, IModalWindowHandler
{
    public Transform InventoryItemsParent;
    public PropItemVm PropItemPrefab;

    private readonly List<PropItemVm> _propViewModels;

    [NotNull] [Inject] private readonly IPlayerState _playerState;
    [NotNull] [Inject] private readonly DiContainer _diContainer;

    private ITrader _trader;

    public event EventHandler Closed;

    public TraderModalBody()
    {
        _propViewModels = new List<PropItemVm>();
    }

    public string Caption { get => "Trader"; }

    public void Init(ITrader trader)
    {
        _trader = trader ?? throw new ArgumentNullException(nameof(trader));

        UpdateProps();
    }

    public void ApplyChanges()
    {

    }

    public void CancelChanges()
    {

    }

    private void UpdateProps()
    {
        foreach (Transform propTransform in InventoryItemsParent)
        {
            var propViewModel = propTransform.GetComponent<PropItemVm>();
            propViewModel.Click -= PropItemViewModel_Click;

            Destroy(propTransform.gameObject);
        }

        var actorViewModel = _playerState.ActiveActor;
        var inventory = actorViewModel.Actor.Person.Inventory;
        var props = inventory.CalcActualItems();

        foreach (var prop in props)
        {
            CreatePropObject(InventoryItemsParent, prop);
        }
    }

    private void CreatePropObject(Transform itemsParent, IProp prop)
    {
        var propItemViewModel = Instantiate(PropItemPrefab, itemsParent);
        propItemViewModel.Init(prop);
        propItemViewModel.Click += PropItemViewModel_Click;
        _propViewModels.Add(propItemViewModel);
    }

    private void PropItemViewModel_Click(object sender, EventArgs e)
    {
        var propViewModel = (IPropItemViewModel)sender;

        var actorViewModel = _playerState.ActiveActor;
        var inventory = actorViewModel.Actor.Person.Inventory;

        switch (propViewModel.Prop)
        {
            case Resource resource:
                inventory.Remove(new Resource(resource.Scheme, 1));
                break;

            case Equipment _:
            case Concept _:
                inventory.Remove(propViewModel.Prop);
                break;

            default:
                throw new InvalidOperationException();
        }
        

        var goods = _trader.Offer();

        inventory.Add(goods);

        UpdateProps();
    }
}