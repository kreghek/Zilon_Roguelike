using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Assets.Zilon.Scripts.Models;
using Assets.Zilon.Scripts.Models.Modals;

using JetBrains.Annotations;

using UnityEngine;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Props;
using Zilon.Core.Tactics;

public class InventoryHandler : MonoBehaviour
{
    private const string HISTORY_BOOK_SID = "history-book";

    private IActor _actor;
    private readonly List<PropItemVm> _propViewModels;
    private TaskScheduler _taskScheduler;

    public Transform InventoryItemsParent;
    public PropItemVm PropItemPrefab;
    public Transform EquipmentSlotsParent;
    public InventorySlotVm EquipmentSlotPrefab;
    public PropInfoPopup PropInfoPopup;
    public GameObject UseButton;
    public GameObject ReadButton;
    public GameObject UsePropDropArea;
    public GameObject DeequipPropDropArea;
    public SectorCommandContextFactory SectorCommandContextFactory;

    [NotNull] [Inject] private readonly DiContainer _diContainer;
    [NotNull] [Inject] private readonly ISectorUiState _sectorUiState;
    [NotNull] [Inject] private readonly IInventoryState _inventoryState;
    [NotNull] [Inject] private readonly ICommandManager<SectorCommandContext> _commandManager;
    [NotNull] [Inject] private readonly ICommandManager<ActorModalCommandContext> _modalCommandManager;
    [NotNull] [Inject(Id = "use-self-command")] private readonly ICommand<SectorCommandContext> _useSelfCommand;
    [NotNull] [Inject(Id = "show-history-command")] private readonly ICommand<ActorModalCommandContext> _showHistoryCommand;

    public event EventHandler Closed;

    public InventoryHandler()
    {
        _propViewModels = new List<PropItemVm>();
    }

    public void Start()
    {
        if (SectorCommandContextFactory == null)
        {
            throw new InvalidOperationException($"Не задан {nameof(SectorCommandContextFactory)}");
        }

        _taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

        _sectorUiState.ActiveActorChanged += SectorUiState_ActiveActorChangedAsync;
    }

    private async void SectorUiState_ActiveActorChangedAsync(object sender, EventArgs e)
    {
        var newPerson = _sectorUiState.ActiveActor.Actor;
        HandlePersonChanged(newPerson);

        // Этот код обработчика должен выполниться в потоке Unity и не важно в каком потоке было выстелено событие.
        // https://stackoverflow.com/questions/40733647/how-to-call-event-handler-through-ui-thread-when-the-operation-is-executing-into
        await Task.Factory.StartNew(() =>
        {
            CreateSlots();
            StartUpControls();

            var inventory = _actor.Person.Inventory;
            UpdatePropsInner(InventoryItemsParent, inventory.CalcActualItems());

            inventory.Added += Inventory_Added;
            inventory.Removed += Inventory_Removed;
            inventory.Changed += Inventory_Changed;

            _inventoryState.SelectedPropChanged += InventoryState_SelectedPropChanged;
        }, CancellationToken.None, TaskCreationOptions.None, _taskScheduler);
    }

    private void HandlePersonChanged(IActor newPerson)
    {
        if (newPerson == _actor)
        {
            return;
        }

        DropCurrentPersonSubscribtions();
        _actor = newPerson;
    }

    private void DropCurrentPersonSubscribtions()
    {

    }

    /// <summary>
    /// Первоначальная настройка всех элементов UI.
    /// Приводим к первоначальному виду, чтобы было сложнее забыть что-нибудь отключить/скрыть
    /// во время разработки.
    /// </summary>
    private void StartUpControls()
    {
        // Изначально скрываем все кнопки.
        // Потому что изначально никакой предмет не должен быть выбран.
        // Поэтому не ясно, какие действия доступны.
        UseButton.SetActive(false);
        ReadButton.SetActive(false);

        // Скрываем все области сброса.
        // Потому что изначально никто никакие предметы не перетаскивает.
        DeequipPropDropArea.SetActive(false);
        UsePropDropArea.SetActive(false);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _inventoryState.SelectedProp = null;
        }
    }

    private void InventoryState_SelectedPropChanged(object sender, EventArgs e)
    {
        foreach (var propViewModel in _propViewModels)
        {
            var isSelected = ReferenceEquals(propViewModel, _inventoryState.SelectedProp);
            propViewModel.SetSelectedState(isSelected);
        }

        PropInfoPopup.SetPropViewModel(_inventoryState.SelectedProp as IPropViewModelDescription);

        UpdateUseControlsState(_inventoryState.SelectedProp as PropItemVm);
    }

    public void OnDestroy()
    {
        var inventory = _actor.Person.Inventory;

        inventory.Added -= Inventory_Added;
        inventory.Removed -= Inventory_Removed;
        inventory.Changed -= Inventory_Changed;

        _inventoryState.SelectedPropChanged -= InventoryState_SelectedPropChanged;
        _inventoryState.SelectedProp = null;
    }

    private void CreateSlots()
    {
        var slots = _actor.Person.EquipmentCarrier.Slots;

        for (var i = 0; i < slots.Length; i++)
        {
            var slotObject = _diContainer.InstantiatePrefab(EquipmentSlotPrefab, EquipmentSlotsParent);
            var slotViewModel = slotObject.GetComponent<InventorySlotVm>();
            slotViewModel.SectorCommandContextFactory = SectorCommandContextFactory;
            slotViewModel.Actor = _actor;
            slotViewModel.SlotIndex = i;
            slotViewModel.SlotTypes = slots[i].Types;
            slotViewModel.Click += Slot_Click;
            slotViewModel.MouseEnter += SlotViewModel_MouseEnter;
            slotViewModel.MouseExit += SlotViewModel_MouseExit;
            slotViewModel.DraggingStateChanged += SlotViewModel_DraggingStateChanged;
        }
    }

    private void SlotViewModel_DraggingStateChanged(object sender, PropDraggingStateEventArgs e)
    {
        DeequipPropDropArea.SetActive(e.Dragging);
    }

    private void SlotViewModel_MouseExit(object sender, EventArgs e)
    {
        PropInfoPopup.SetPropViewModel(null);
    }

    private void SlotViewModel_MouseEnter(object sender, EventArgs e)
    {
        var currentItemVm = (IPropViewModelDescription)sender;
        PropInfoPopup.SetPropViewModel(currentItemVm);
    }

    private void Slot_Click(object sender, EventArgs e)
    {
        var slotVm = sender as InventorySlotVm;
        if (slotVm == null)
        {
            throw new NotSupportedException();
        }

        slotVm.ApplyEquipment();
    }

    private async void Inventory_Removed(object sender, PropStoreEventArgs e)
    {
        // Этот код обработчика должен выполниться в потоке Unity и не важно в каком потоке было выстелено событие.
        // https://stackoverflow.com/questions/40733647/how-to-call-event-handler-through-ui-thread-when-the-operation-is-executing-into
        await Task.Factory.StartNew(() =>
        {
            foreach (var removedProp in e.Props)
            {
                var propViewModel = _propViewModels.Single(x => x.Prop == removedProp);
                _propViewModels.Remove(propViewModel);
                Destroy(propViewModel.gameObject);

                var isRemovedPropWasSelected = ReferenceEquals(propViewModel, _inventoryState.SelectedProp);
                if (isRemovedPropWasSelected)
                {
                    _inventoryState.SelectedProp = null;
                    UseButton.SetActive(false);
                }
            }

            UpdateItemsParentObject();
        }, CancellationToken.None, TaskCreationOptions.None, _taskScheduler);
    }

    private async void Inventory_Changed(object sender, PropStoreEventArgs e)
    {
        // Этот код обработчика должен выполниться в потоке Unity и не важно в каком потоке было выстелено событие.
        // https://stackoverflow.com/questions/40733647/how-to-call-event-handler-through-ui-thread-when-the-operation-is-executing-into
        await Task.Factory.StartNew(() =>
        {
            foreach (var changedProp in e.Props)
            {
                var propViewModel = _propViewModels.Single(x => x.Prop == changedProp);
                propViewModel.UpdateProp();
            }
        }, CancellationToken.None, TaskCreationOptions.None, _taskScheduler);
    }

    private async void Inventory_Added(object sender, PropStoreEventArgs e)
    {
        // Этот код обработчика должен выполниться в потоке Unity и не важно в каком потоке было выстелено событие.
        // https://stackoverflow.com/questions/40733647/how-to-call-event-handler-through-ui-thread-when-the-operation-is-executing-into
        await Task.Factory.StartNew(() =>
        {
            foreach (var newProp in e.Props)
            {
                CreatePropObject(InventoryItemsParent, newProp);
            }

            UpdateItemsParentObject();
        }, CancellationToken.None, TaskCreationOptions.None, _taskScheduler);
    }

    private void UpdateItemsParentObject()
    {
        var inventory = _actor.Person.Inventory;
        var inventoryProps = inventory.CalcActualItems();
        RecalcItemsObject(InventoryItemsParent, inventoryProps);
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

        RecalcItemsObject(itemsParent, props);
    }

    private static void RecalcItemsObject(Transform itemsParent, IEnumerable<IProp> props)
    {
        var parentRect = itemsParent.GetComponent<RectTransform>();
        var rowCount = (int)Math.Ceiling(props.Count() / 4f);
        parentRect.sizeDelta = new Vector2(parentRect.sizeDelta.x, (40 + 5) * rowCount);
    }

    private void CreatePropObject(Transform itemsParent, IProp prop)
    {
        var propItemViewModelObj = _diContainer.InstantiatePrefab(PropItemPrefab, itemsParent);

        var propItemViewModel = propItemViewModelObj.GetComponent<PropItemVm>();
        propItemViewModel.Init(prop);
        propItemViewModel.Click += PropItem_Click;
        //TODO Переделать
        //
        propItemViewModel.DraggingStateChanged += PropItemViewModel_DraggingStateChanged;
        propItemViewModel.MouseEnter += PropItemViewModel_MouseEnter;
        propItemViewModel.MouseExit += PropItemViewModel_MouseExit;
        _propViewModels.Add(propItemViewModel);
    }

    private void PropItemViewModel_DraggingStateChanged(object sender, PropDraggingStateEventArgs e)
    {
        var currentItemViewModel = (PropItemVm)sender;
        foreach (var propViewModel in _propViewModels)
        {
            var isSelected = propViewModel == currentItemViewModel;
            propViewModel.SetSelectedState(isSelected);
        }

        UpdateUseControlsState(currentItemViewModel);
    }

    private void PropItemViewModel_MouseExit(object sender, EventArgs e)
    {
        PropInfoPopup.SetPropViewModel(null);
    }

    private void PropItemViewModel_MouseEnter(object sender, EventArgs e)
    {
        var currentItemVm = (PropItemVm)sender;
        PropInfoPopup.SetPropViewModel(currentItemVm);
    }

    //TODO Дубликат с ContainerModalBody.PropItemOnClick
    private void PropItem_Click(object sender, EventArgs e)
    {
        var currentItemViewModel = (PropItemVm)sender;
        foreach (var propViewModel in _propViewModels)
        {
            var isSelected = propViewModel == currentItemViewModel;
            propViewModel.SetSelectedState(isSelected);
        }

        // этот фрагмент - не дубликат
        UpdateUseControlsState(currentItemViewModel);

        // В сервисе InventoryState указываем, что текущий предмет выбран.
        // Текущий - это тот, на который только что кликнули.
        // Если он уже выбран, то сбрасываем выделение.
        if (!ReferenceEquals(_inventoryState.SelectedProp, currentItemViewModel))
        {
            _inventoryState.SelectedProp = currentItemViewModel;
        }
        else
        {
            _inventoryState.SelectedProp = null;
        }

        // --- этот фрагмент - не дубликат
    }

    private void UpdateUseControlsState(PropItemVm currentItemViewModel)
    {
        if (currentItemViewModel?.Prop == null)
        {
            UseButton.SetActive(false);
            ReadButton.SetActive(false);
            return;
        }

        if (currentItemViewModel.SelectAsDrag && currentItemViewModel.Prop.Scheme.Use != null)
        {
            UseButton.SetActive(false);
            ReadButton.SetActive(false);

            UsePropDropArea.SetActive(true);
        }
        else
        {
            UsePropDropArea.SetActive(false);

            var currentItem = currentItemViewModel.Prop;

            var canUseProp = currentItem.Scheme.Use != null;
            UseButton.SetActive(canUseProp);

            var canRead = currentItem.Scheme.Sid == HISTORY_BOOK_SID;
            ReadButton.SetActive(canRead);
        }
    }

    public void UseButton_Handler()
    {
        _commandManager.Push(_useSelfCommand);
    }

    public void ReadButton_Handler()
    {
        if (_inventoryState.SelectedProp.Prop.Scheme.Sid == HISTORY_BOOK_SID)
        {
            _modalCommandManager.Push(_showHistoryCommand);
        }
    }
}
