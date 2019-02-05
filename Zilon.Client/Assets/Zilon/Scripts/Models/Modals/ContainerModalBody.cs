using System;
using System.Collections.Generic;
using System.Linq;

using Assets.Zilon.Scripts;

using JetBrains.Annotations;

using UnityEngine;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Props;
using Zilon.Core.Tactics;

// ReSharper disable once ClassNeverInstantiated.Global
// ReSharper disable once CheckNamespace
public class ContainerModalBody : MonoBehaviour, IModalWindowHandler
{
    private List<PropItemVm> _inventoryViewModels;
    private List<PropItemVm> _containerViewModels;

    // ReSharper disable NotNullMemberIsNotInitialized
    // ReSharper disable UnassignedField.Global
    // ReSharper disable MemberCanBePrivate.Global
#pragma warning disable 649
    [NotNull] public PropItemVm PropItemPrefab;

    [NotNull] public Transform InventoryItemsParent;

    [NotNull] public Transform ContainerItemsParent;

    // ReSharper restore MemberCanBePrivate.Global

    [NotNull] [Inject] private readonly ICommandManager _clientCommandExecutor;

    [NotNull] [Inject] private readonly IPlayerState _playerState;

    [NotNull] [Inject] private readonly ISectorManager _sectorManger;

    [NotNull] [Inject] private readonly IGameLoop _gameLoop;

    [NotNull] [Inject(Id = "prop-transfer-command")] private readonly ICommand _propTransferCommand;

    [NotNull] private PropTransferMachine _transferMachine;

    public event EventHandler Closed;

    public string Caption => "Loot";

#pragma warning restore 649
    // ReSharper restore UnassignedField.Global
    // ReSharper restore NotNullMemberIsNotInitialized

    public void Init(PropTransferMachine transferMachine)
    {
        _inventoryViewModels = new List<PropItemVm>();
        _containerViewModels = new List<PropItemVm>();

        _transferMachine = transferMachine;

        ((PropTransferCommand)_propTransferCommand).TransferMachine = transferMachine;

        UpdateProps();
    }

    private void UpdateProps()
    {
        var inventoryItems = _transferMachine.Inventory.CalcActualItems();
        UpdatePropsInner(InventoryItemsParent, inventoryItems, InventoryPropItem_Click, _inventoryViewModels);
        _transferMachine.Inventory.Added += Inventory_Added;
        _transferMachine.Inventory.Removed += Inventory_Removed;

        var containerItems = _transferMachine.Container.CalcActualItems();
        UpdatePropsInner(ContainerItemsParent, containerItems, ContainerPropItem_Click, _containerViewModels);
        _transferMachine.Container.Added += Container_Added;
        _transferMachine.Container.Removed += Container_Removed;
    }

    private void Container_Removed(object sender, PropStoreEventArgs e)
    {
        var propStore = (IPropStore)sender;
        var itemsParent = InventoryItemsParent;

        foreach (var prop in e.Props)
        {
            PropItemVm propViewModel;
            switch (prop)
            {
                case Resource resource:
                    propViewModel = _containerViewModels.Single(x => x.Prop.Scheme == prop.Scheme);
                    break;

                case Equipment _:
                case Concept _:
                    propViewModel = _containerViewModels.Single(x => x.Prop == prop);
                    break;

                default:
                    throw new InvalidOperationException();
            }
            
            _containerViewModels.Remove(propViewModel);
            Destroy(propViewModel.gameObject);
            propViewModel.Click -= ContainerPropItem_Click;
        }

        var parentRect = itemsParent.GetComponent<RectTransform>();
        var rowCount = (int)Math.Ceiling(propStore.CalcActualItems().Count() / 4f);
        parentRect.sizeDelta = new Vector2(parentRect.sizeDelta.x, (40 + 5) * rowCount);
    }

    private void Container_Added(object sender, PropStoreEventArgs e)
    {
        var propStore = (IPropStore)sender;
        var itemsParent = InventoryItemsParent;

        foreach (var prop in e.Props)
        {
            var propItemViewModel = Instantiate(PropItemPrefab, itemsParent);
            propItemViewModel.Init(prop);
            propItemViewModel.Click += ContainerPropItem_Click;
            _containerViewModels.Add(propItemViewModel);
        }

        var parentRect = itemsParent.GetComponent<RectTransform>();
        var rowCount = (int)Math.Ceiling(propStore.CalcActualItems().Count() / 4f);
        parentRect.sizeDelta = new Vector2(parentRect.sizeDelta.x, (40 + 5) * rowCount);
    }

    private void Inventory_Removed(object sender, PropStoreEventArgs e)
    {
        var propStore = (IPropStore)sender;
        var itemsParent = InventoryItemsParent;

        foreach (var prop in e.Props)
        {
            PropItemVm propViewModel;
            switch (prop)
            {
                case Resource resource:
                    propViewModel = _containerViewModels.Single(x => x.Prop.Scheme == prop.Scheme);
                    break;

                case Equipment _:
                case Concept _:
                    propViewModel = _containerViewModels.Single(x => x.Prop == prop);
                    break;

                default:
                    throw new InvalidOperationException();
            }

            _inventoryViewModels.Remove(propViewModel);
            Destroy(propViewModel.gameObject);
            propViewModel.Click -= InventoryPropItem_Click;
        }

        var parentRect = itemsParent.GetComponent<RectTransform>();
        var rowCount = (int)Math.Ceiling(propStore.CalcActualItems().Count() / 4f);
        parentRect.sizeDelta = new Vector2(parentRect.sizeDelta.x, (40 + 5) * rowCount);
    }

    private void Inventory_Added(object sender, PropStoreEventArgs e)
    {
        var propStore = (IPropStore)sender;
        var itemsParent = InventoryItemsParent;

        foreach (var prop in e.Props)
        {
            var propItemViewModel = Instantiate(PropItemPrefab, itemsParent);
            propItemViewModel.Init(prop);
            propItemViewModel.Click += InventoryPropItem_Click;
            _inventoryViewModels.Add(propItemViewModel);
        }

        var parentRect = itemsParent.GetComponent<RectTransform>();
        var rowCount = (int)Math.Ceiling(propStore.CalcActualItems().Count() / 4f);
        parentRect.sizeDelta = new Vector2(parentRect.sizeDelta.x, (40 + 5) * rowCount);
    }

    private void UpdatePropsInner(Transform itemsParent,
        IEnumerable<IProp> props,
        EventHandler propItemHandler,
        List<PropItemVm> propItems)
    {
        foreach (var prop in props)
        {
            var propItemVm = Instantiate(PropItemPrefab, itemsParent);
            propItemVm.Init(prop);
            propItemVm.Click += propItemHandler;
            propItems.Add(propItemVm);
        }

        var parentRect = itemsParent.GetComponent<RectTransform>();
        var rowCount = (int)Math.Ceiling(props.Count() / 4f);
        parentRect.sizeDelta = new Vector2(parentRect.sizeDelta.x, (40 + 5) * rowCount);
    }

    private void InventoryPropItem_Click(object sender, EventArgs e)
    {
        var currentItemViewModel = (PropItemVm)sender;
        _transferMachine.TransferProp(currentItemViewModel.Prop,
            PropTransferMachineStores.Inventory,
            PropTransferMachineStores.Container);
    }

    private void ContainerPropItem_Click(object sender, EventArgs e)
    {
        var currentItemViewModel = (PropItemVm)sender;
        _transferMachine.TransferProp(currentItemViewModel.Prop,
            PropTransferMachineStores.Container,
            PropTransferMachineStores.Inventory);
    }

    // ReSharper disable once UnusedMember.Global
    public void TakeAllButton_Click()
    {
        var props = _transferMachine.Container.CalcActualItems();
        foreach (var prop in props)
        {
            _transferMachine.TransferProp(prop,
                PropTransferMachineStores.Container,
                PropTransferMachineStores.Inventory);
        }

        Closed?.Invoke(this, new EventArgs());
    }

    public void ApplyChanges()
    {
        _clientCommandExecutor.Push(_propTransferCommand);
    }

    public void CancelChanges()
    {
        throw new NotImplementedException();
    }

    public void OnDestroy()
    {
        foreach (Transform propTranfsorm in InventoryItemsParent)
        {
            var propItemViewModel = propTranfsorm.GetComponent<PropItemVm>();
            propItemViewModel.Click -= InventoryPropItem_Click;
            Destroy(propItemViewModel.gameObject);
        }

        foreach (Transform propTranfsorm in ContainerItemsParent)
        {
            var propItemViewModel = propTranfsorm.GetComponent<PropItemVm>();
            propItemViewModel.Click -= ContainerPropItem_Click;
            Destroy(propItemViewModel.gameObject);
        }

        _transferMachine.Inventory.Added -= Inventory_Added;
        _transferMachine.Inventory.Removed -= Inventory_Removed;

        _transferMachine.Container.Added -= Container_Added;
        _transferMachine.Container.Removed -= Container_Removed;
    }
}
