using Assets.Zilon.Scripts.Services;

using UnityEngine.EventSystems;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Commands;

public class InventorySlotDropHandler : UIBehaviour, IDropHandler
{
    [Inject] private readonly SpecialCommandManager _specialCommandManager;
    [Inject] private readonly ICommandManager<SectorCommandContext> _commandManager;
    [Inject] private readonly IInventoryState _inventoryState;

    public InventorySlotVm InventorySlotViewModel;
    public SectorCommandContextFactory SectorCommandContextFactory;

    public void OnDrop(PointerEventData eventData)
    {
        var slotIndex = InventorySlotViewModel.SlotIndex;

        var equipCommand = _specialCommandManager.GetEquipCommand(slotIndex);

        var commandContext = SectorCommandContextFactory.CreateContext();

        if (equipCommand.CanExecute(commandContext))
        {
            var droppedPropItem = eventData.pointerDrag?.GetComponent<PropItemVm>();
            if (droppedPropItem != null)
            {
                // Значит экипировка произошла из инвентаря.
                _inventoryState.SelectedProp = droppedPropItem;

                _commandManager.Push(equipCommand);

                return;
            }

            var droppedInventorySlot = eventData.pointerDrag?.GetComponent<InventorySlotVm>();
            if (droppedInventorySlot != null)
            {
                _inventoryState.SelectedProp = droppedInventorySlot;

                _commandManager.Push(equipCommand);

                return;
            }
        }
    }
}
