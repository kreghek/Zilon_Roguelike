using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Zilon.Scripts;

using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Props;
using Zilon.Core.Tactics;

public class InventoryModalBody : MonoBehaviour, IModalWindowHandler
{
    private IActor _actor;
    private readonly List<PropItemVm> _propViewModels;

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

    public InventoryModalBody()
    {
        _propViewModels = new List<PropItemVm>();
    }

    public string Caption => "Inventory";

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
            slotViewModel.SlotTypes = slots[i].Types;
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
        // изначально скрываем кнопку использования
        UseButton.SetActive(false);

        _actor = actor;
        var inventory = _actor.Person.Inventory;
        UpdatePropsInner(InventoryItemsParent, inventory.CalcActualItems());

        inventory.Added += Inventory_Added;
        inventory.Removed += InventoryOnContentChanged;
        inventory.Changed += Inventory_Changed;
    }

    private void Inventory_Changed(object sender, PropStoreEventArgs e)
    {
        var inventory = _actor.Person.Inventory;
        foreach (var changedProp in e.Props)
        {
            var propViewModel = _propViewModels.Single(x=>x.Prop == changedProp);
            propViewModel.UpdateProp();
        }
    }

    private void Inventory_Added(object sender, PropStoreEventArgs e)
    {
        var inventory = _actor.Person.Inventory;
        foreach (var newProp in e.Props)
        {
            CreatePropObject(InventoryItemsParent, newProp);
        }
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

        _inventoryState.SelectedProp = null;
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
            CreatePropObject(itemsParent, prop);
        }
    }

    private void CreatePropObject(Transform itemsParent, IProp prop)
    {
        var propItemViewModel = Instantiate(PropItemPrefab, itemsParent);
        propItemViewModel.Init(prop);
        propItemViewModel.Click += PropItemOnClick;
        _propViewModels.Add(propItemViewModel);
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

        var propTitle = currentItemVm.Prop.Scheme.Name.En ?? currentItemVm.Prop.Scheme.Name.Ru;
        DetailText.text = propTitle;
        // --- этот фрагмент - не дубликат

        _inventoryState.SelectedProp = currentItemVm;
    }

    public void UseButton_Handler()
    {
        _commandManager.Push(_useSelfCommand);
    }
}
