using System.Collections.Generic;
using UnityEngine;
using Zilon.Core.Client;
using Zilon.Core.Persons;

public class ShowContainerModalBody : MonoBehaviour
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

	public void UpdateProps()
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
}
