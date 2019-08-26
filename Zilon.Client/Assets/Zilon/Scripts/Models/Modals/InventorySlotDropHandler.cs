using Assets.Zilon.Scripts.Services;

using UnityEngine;
using UnityEngine.EventSystems;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Commands;

public class InventorySlotDropHandler : UIBehaviour, IDropHandler
{
    [Inject] private readonly SpecialCommandManager _specialCommandManager;
    [Inject] private readonly ICommandManager _commandManager;
    [Inject] private readonly IInventoryState _inventoryState;

    public InventorySlotVm InventorySlotViewModel;

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("1234");

        var slotIndex = InventorySlotViewModel.SlotIndex;

        var equipCommand = _specialCommandManager.GetEquipCommand(slotIndex);

        if (equipCommand.CanExecute())
        {
            var droppedPropItem = eventData.pointerDrag?.GetComponent<PropItemVm>();
            _inventoryState.SelectedProp = droppedPropItem;

            _commandManager.Push(equipCommand);
        }
    }
}
