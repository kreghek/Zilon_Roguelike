using Assets.Zilon.Scripts;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zilon.Core.Client;
using Zilon.Core.Persons;

public class ShowContainerModalBody : MonoBehaviour, IModalWindowHandler
{
    private PropTransferMachine _transferMachine;

    public PropItemVm PropItemPrefab;
    public Transform InventoryItemsParent;
    public Transform ContainerItemsParent;

    public void SetTransferMachine(PropTransferMachine transferMachine)
    {
        _transferMachine = transferMachine;
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
        }
    }

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
        Debug.Log(_transferMachine.Inventory.PropAdded);
    }

    public void CancelChanges()
    {
        throw new System.NotImplementedException();
    }
}
