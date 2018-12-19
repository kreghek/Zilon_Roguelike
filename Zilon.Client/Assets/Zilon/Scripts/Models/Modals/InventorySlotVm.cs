using System;

using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Persons;
using Zilon.Core.Tactics;

public class InventorySlotVm : MonoBehaviour
{
    public static int Count;

    public int CurrentCount;

    [NotNull] [Inject] private readonly ISectorManager _sectorManager;
    [NotNull] [Inject] private readonly ICommandManager _comamndManager;
    [NotNull] [Inject] private readonly IInventoryState _inventoryState;
    [NotNull] [Inject] private readonly IPlayerState _playerState;
    [NotNull] [Inject(Id = "equip-command")] private readonly ICommand _equipCommand;

    public int SlotIndex;
    public GameObject Border;
    public Image IconImage;

    public event EventHandler Click;

    public void Start()
    {
        CurrentCount = Count;
        Count++;

        ((EquipCommand)_equipCommand).SlotIndex = SlotIndex;

        UpdateSlotIcon();
        InitEventHandlers();
    }

    public void OnDestroy()
    {
        ClearEventHandlers();
    }

    private void EquipmentCarrierOnEquipmentChanged(object sender, EquipmentChangedEventArgs e)
    {
        UpdateSlotIcon();
        _inventoryState.SelectedProp = null;
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
        _comamndManager.Push(_equipCommand);
    }


    private void InitEventHandlers()
    {
        var actor = _playerState.ActiveActor.Actor;
        actor.Person.EquipmentCarrier.EquipmentChanged += EquipmentCarrierOnEquipmentChanged;
    }

    private void ClearEventHandlers()
    {
        var actor = _playerState.ActiveActor.Actor;
        actor.Person.EquipmentCarrier.EquipmentChanged -= EquipmentCarrierOnEquipmentChanged;
    }
}