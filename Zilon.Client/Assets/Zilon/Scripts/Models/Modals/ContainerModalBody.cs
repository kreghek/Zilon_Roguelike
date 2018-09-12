using System;
using Assets.Zilon.Scripts;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;
using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Persons;
using Zilon.Core.Tactics;

// ReSharper disable once ClassNeverInstantiated.Global
// ReSharper disable once CheckNamespace
public class ContainerModalBody : MonoBehaviour, IModalWindowHandler
{
    // ReSharper disable NotNullMemberIsNotInitialized
    // ReSharper disable UnassignedField.Global
    // ReSharper disable MemberCanBePrivate.Global
#pragma warning disable 649
    [NotNull] public PropItemVm PropItemPrefab;

    [NotNull] public Transform InventoryItemsParent;

    [NotNull] public Transform ContainerItemsParent;

    // ReSharper restore MemberCanBePrivate.Global

    [NotNull] [Inject] private ICommandManager _clientCommandExecutor;

    [NotNull] [Inject] private IPlayerState _playerState;

    [NotNull] [Inject] private ISectorManager _sectorManger;

    [NotNull] [Inject] private IGameLoop _gameLoop;

    [NotNull] [Inject(Id = "prop-transfer-command")] private ICommand _propTransferCommand;

    [NotNull] private PropTransferMachine _transferMachine;

#pragma warning restore 649    
    // ReSharper restore UnassignedField.Global
    // ReSharper restore NotNullMemberIsNotInitialized

    public void Init(PropTransferMachine transferMachine)
    {
        _transferMachine = transferMachine;

        ((PropTransferCommand)_propTransferCommand).TransferMachine = transferMachine;
        
        UpdateProps();
    }

    private void UpdateProps()
    {
        var inventoryItems = _transferMachine.Inventory.CalcActualItems();
        UpdatePropsInner(InventoryItemsParent, inventoryItems);

        var containerItems = _transferMachine.Container.CalcActualItems();
        UpdatePropsInner(ContainerItemsParent, containerItems);
    }

    private void UpdatePropsInner(Transform itemsParent, IEnumerable<IProp> props)
    {
        foreach (Transform itemTranform in itemsParent)
        {
            Destroy(itemTranform.gameObject);
        }

        foreach (var prop in props)
        {
            var propItemVm = Instantiate(PropItemPrefab, itemsParent);
            propItemVm.Init(prop);
            propItemVm.Click += PropItemOnClick;
        }
    }

    private void PropItemOnClick(object sender, EventArgs e)
    {
        var currentItemVm = (PropItemVm) sender;
        var parentTransform = currentItemVm.transform.parent;
        foreach (Transform itemTranform in parentTransform)
        {
            var itemVm = itemTranform.gameObject.GetComponent<PropItemVm>();
            var isSelected = itemVm == currentItemVm;
            itemVm.SetSelectedState(isSelected);
        }
    }

    // ReSharper disable once UnusedMember.Global
    public void TakeAllButton_Click()
    {
        var props = _transferMachine.Container.CalcActualItems();
        foreach (var prop in props)
        {
            _transferMachine.TransferProp(prop, _transferMachine.Container, _transferMachine.Inventory);
        }

        UpdateProps();
    }

    public void ApplyChanges()
    {
        _clientCommandExecutor.Push(_propTransferCommand);
    }

    public void CancelChanges()
    {
        throw new System.NotImplementedException();
    }
}
