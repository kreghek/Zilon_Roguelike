using Assets.Zilon.Scripts.Services;

using UnityEngine.EventSystems;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Commands;

/// <summary>
/// Обработчик сброса экипировки из слота в инвентарь.
/// </summary>
public class InventoryDropHandler : UIBehaviour, IDropHandler
{
    [Inject] private readonly SpecialCommandManager _specialCommandManager;
    [Inject] private readonly IInventoryState _inventoryState;
    [Inject] private readonly ICommandPool _comamndPool;

    public void OnDrop(PointerEventData eventData)
    {
        var slotViewModelObject = eventData.pointerDrag;
        if (slotViewModelObject is null)
        {
            return;
        }

        var droppedInventorySlot = slotViewModelObject.GetComponent<InventorySlotVm>();
        if (droppedInventorySlot != null)
        {
            var equipCommand = _specialCommandManager.GetEquipCommand(droppedInventorySlot.SlotIndex);

            _inventoryState.SelectedProp = null;

            _comamndPool.Push(equipCommand);

            return;
        }
    }
}
