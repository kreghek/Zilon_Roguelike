using System.Collections.Generic;
using Assets.Zilon.Scripts;
using UnityEngine;
using Zilon.Core.Persons;
using Zilon.Core.Tactics;

public class InventoryModalBody : MonoBehaviour, IModalWindowHandler
{
	public Transform InventoryItemsParent;
	public GameObject PropItemPrefab;

	public void Init(IActor actor)
	{
		var inventory = actor.Inventory;
		UpdatePropsInner(InventoryItemsParent, inventory.Items);
	}

	public void ApplyChanges()
	{
		throw new System.NotImplementedException();
	}

	public void CancelChanges()
	{
		throw new System.NotImplementedException();
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
