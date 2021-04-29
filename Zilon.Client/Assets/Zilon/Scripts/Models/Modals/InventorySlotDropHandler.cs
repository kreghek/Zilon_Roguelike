using Assets.Zilon.Scripts.Services;

using UnityEngine.EventSystems;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Commands;

public class InventorySlotDropHandler : UIBehaviour, IDropHandler
{
    [Inject] private readonly SpecialCommandManager _specialCommandManager;
    [Inject] private readonly ICommandPool _commandPool;
    [Inject] private readonly IInventoryState _inventoryState;

    public InventorySlotVm InventorySlotViewModel;

    public void OnDrop(PointerEventData eventData)
    {
        var slotIndex = InventorySlotViewModel.SlotIndex;

        var equipCommand = _specialCommandManager.GetEquipCommand(slotIndex);

        if (equipCommand.CanExecute().IsSuccess)
        {
            var propItemViewModelObject = eventData.pointerDrag;
            if (propItemViewModelObject is null)
            {
                return;
            }

            var droppedPropItem = propItemViewModelObject.GetComponent<PropItemVm>();
            if (droppedPropItem != null)
            {
                // Значит экипировка произошла из инвентаря.
                _inventoryState.SelectedProp = droppedPropItem;

                _commandPool.Push(equipCommand);

                return;
            }

            var droppedInventorySlot = propItemViewModelObject.GetComponent<InventorySlotVm>();
            if (droppedInventorySlot != null)
            {
                _inventoryState.SelectedProp = droppedInventorySlot;

                _commandPool.Push(equipCommand);

                return;
            }
        }
    }
}
