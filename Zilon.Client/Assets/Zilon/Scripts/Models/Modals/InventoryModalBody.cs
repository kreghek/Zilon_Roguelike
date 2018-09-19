using System;
using System.Collections.Generic;

using Assets.Zilon.Scripts;

using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Persons;
using Zilon.Core.Tactics;

public class InventoryModalBody : MonoBehaviour, IModalWindowHandler
{
    private IActor _actor;

    public Transform InventoryItemsParent;
    public PropItemVm PropItemPrefab;
    public Transform EquipmentSlotsParent;
    public InventorySlotVm EquipmentSlotPrefab;
    public GameObject UseButton;
    public Text DetailText;

    [NotNull] [Inject] private DiContainer _diContainer;
    [NotNull] [Inject] private IPlayerState _playerState;
    [NotNull] [Inject] private IInventoryState _inventoryState;
    [NotNull] [Inject] private ICommandManager _commandManager;
    [NotNull] [Inject(Id = "use-self-command")] private readonly ICommand _useSelfCommand;

    public void Start()
    {
        CreateSlots();
    }

    private void CreateSlots()
    {
        var actorViewModel = _playerState.ActiveActor;
        var slots = actorViewModel.Actor.Person.EquipmentCarrier.Slots;

        for (var i = 0; i < slots.Length; i++)
        {
            var slotObject = _diContainer.InstantiatePrefab(EquipmentSlotPrefab, EquipmentSlotsParent);
            var slotViewModel = slotObject.GetComponent<InventorySlotVm>();
            slotViewModel.SlotIndex = i;
            slotViewModel.Click += SlotOnClick;
        }
    }

    private void SlotOnClick(object sender, EventArgs e)
    {
        var slotVm = sender as InventorySlotVm;
        if (slotVm == null)
        {
            throw new NotSupportedException();
        }

        slotVm.ApplyEquipment();
    }

    public void Init(IActor actor)
    {
        _actor = actor;
        var inventory = _actor.Person.Inventory;
        UpdatePropsInner(InventoryItemsParent, inventory.CalcActualItems());

        inventory.Added += InventoryOnContentChanged;
        inventory.Removed += InventoryOnContentChanged;
        inventory.Changed += InventoryOnContentChanged;
    }

    private void InventoryOnContentChanged(object sender, PropStoreEventArgs e)
    {
        var inventory = _actor.Person.Inventory;
        UpdatePropsInner(InventoryItemsParent, inventory.CalcActualItems());
    }

    public void ApplyChanges()
    {
        var inventory = _actor.Person.Inventory;
        inventory.Added -= InventoryOnContentChanged;
        inventory.Removed -= InventoryOnContentChanged;
        inventory.Changed -= InventoryOnContentChanged;
    }

    public void CancelChanges()
    {
        throw new NotImplementedException();
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
        var currentItemVm = (PropItemVm)sender;
        var parentTransform = currentItemVm.transform.parent;
        foreach (Transform itemTranform in parentTransform)
        {
            var itemVm = itemTranform.gameObject.GetComponent<PropItemVm>();
            var isSelected = itemVm == currentItemVm;
            itemVm.SetSelectedState(isSelected);
        }

        // этот фрагмент - не дубликат
        var canUseProp = currentItemVm.Prop.Scheme.Use != null;
        UseButton.SetActive(canUseProp);

        var propTitle = currentItemVm.Prop.Scheme.Name.Ru ?? currentItemVm.Prop.Scheme.Name.En;
        DetailText.text = propTitle;
        // --- этот фрагмент - не дубликат

        _inventoryState.SelectedProp = currentItemVm;
    }

    public void UseButton_Handler()
    {
        Debug.Log($"Used: {_inventoryState.SelectedProp.Prop.Scheme.Sid}");
        _commandManager.Push(_useSelfCommand);
    }
}
