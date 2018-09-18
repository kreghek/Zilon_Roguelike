using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Persons;

public class InventorySlotVm : MonoBehaviour
{
    [NotNull] [Inject] private ISectorManager _sectorManager;
    [NotNull] [Inject] private ICommandManager _comamndManager;
    [NotNull] [Inject] private IInventoryState _inventoryState;
    [NotNull] [Inject] private IPlayerState _playerState;
    [NotNull] [Inject(Id = "equip-command")] private ICommand _equipCommand;

    public int SlotIndex;
    public GameObject Border;
    public Image IconImage;

    public event EventHandler Click;

    public void Start()
    {
        ((EquipCommand)_equipCommand).SlotIndex = SlotIndex;

        UpdateSlotIcon();

        var actor = _playerState.ActiveActor.Actor;
        actor.Person.EquipmentCarrier.EquipmentChanged += EquipmentCarrierOnEquipmentChanged;
    }

    private void EquipmentCarrierOnEquipmentChanged(object sender, EquipmentChangedEventArgs e)
    {
        UpdateSlotIcon();
    }

    private void UpdateSlotIcon()
    {
        var actor = _playerState.ActiveActor.Actor;

        var currentEquipment = actor.Person.EquipmentCarrier.Equipments[SlotIndex];
        if (currentEquipment != null)
        {
            if (IconImage != null)
            {
                IconImage.gameObject.SetActive(true);
                IconImage.sprite = Resources.Load<Sprite>($"Icons/props/{currentEquipment.Scheme.Sid}");
            }
        }
        else
        {
            if (IconImage != null)
            {
                IconImage.gameObject.SetActive(false);
            }
        }
    }

    public void FixedUpdate()
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