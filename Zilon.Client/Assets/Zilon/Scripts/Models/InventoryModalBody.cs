using System;
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
			propItemVm.Click += PropItemOnClick;
		}
	}

	//TODO Дубликат с ContainerModalBody.PropItemOnClick
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
}
