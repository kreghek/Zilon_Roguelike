using System;
using Assets.Zilon.Scripts.Models.Commands;
using Assets.Zilon.Scripts.Models.SectorScene;
using Assets.Zilon.Scripts.Services;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;
using Zilon.Core.Commands;

public class InventorySlotVm : MonoBehaviour
{
	[NotNull] [Inject] private ISectorManager _sectorManager;
	[NotNull] [Inject] private ICommandManager _comamndManager;
	[NotNull] [Inject] private IInventoryState _inventoryState;
	[NotNull] [Inject(Id = "equip-command")] private ICommand _equipCommand;

	public int SlotIndex;
	public GameObject Border;
	
	public event EventHandler Click;

	public void Start()
	{
		((EquipCommand) _equipCommand).SlotIndex = SlotIndex;
	}

	public void Update()
	{
		var canEquip = _equipCommand.CanExecute();
		var selectedProp = _inventoryState.SelectedProp;
		var denySlot = !canEquip && selectedProp != null;
		Border.SetActive(denySlot);
	}
	
	public void Click_Handler()
	{
		Click?.Invoke(this, new EventArgs());	
	}

	public void ApplyEquipment()
	{
		Debug.Log($"Slot {SlotIndex} equiped {_inventoryState.SelectedProp.Prop}");
		var currentSector = _sectorManager.CurrentSector;
		
		_equipCommand.Execute();
		_comamndManager.Push(_equipCommand);
		
		currentSector.Update();
	}
}
