using Assets.Zilon.Scripts;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zilon.Core.Client;
using Zilon.Core.Persons;

public class ShowContainerModalBody : MonoBehaviour, IModalWindowHandler
{
    private PropTransferMachine _transferMachine;

    public GameObject PropItemPrefab;
    public Transform InventoryItemsParent;
    public Transform ContainerItemsParent;

    public void SetTransferMachine(PropTransferMachine transferMachine)
    {
        _transferMachine = transferMachine;
        UpdateProps();
    }

    private void UpdateProps()
    {
        UpdatePropsInner(InventoryItemsParent, _transferMachine.Inventory.Items);
        UpdatePropsInner(ContainerItemsParent, _transferMachine.Container.Items);
    }

    private void UpdatePropsInner(Transform itemsParent, IEnumerable<IProp> props)
    {
        foreach (Transform itemTranform in itemsParent)
        {
            Destroy(itemTranform.gameObject);
        }

        foreach (var prop in props)
        {
            Instantiate(PropItemPrefab, itemsParent);
        }
    }

    public void TakeAllButton_Click()
    {
        var props = _transferMachine.Container.Items.ToArray();
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
