using System.Collections.Generic;
using Assets.Zilon.Scripts;
using UnityEngine;
using Zilon.Core.Persons;
using Zilon.Core.Tactics;

public class InventoryModalBody : MonoBehaviour, IModalWindowHandler
{
	public Transform InventoryItemsParent;
	public PropItemVm PropItemPrefab;

	public void Init(IActor actor)
	{
		var inventory = actor.Inventory;
		UpdatePropsInner(InventoryItemsParent, inventory.CalcActualItems());
	}

	public void ApplyChanges()
	{
		
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
			var propItemVm = Instantiate(PropItemPrefab, itemsParent);
			propItemVm.Init(prop);
		}
	}
}
