using Assets.Zilon.Scripts.Models.Modals;

public class DraggedPropItem : PropItemViewModelBase
{
    public void Init(PropItemVm propItemViewModel)
    {
        Prop = propItemViewModel?.Prop;

        UpdateProp();
    }

    public void Init(InventorySlotVm inventorySlotViewModel)
    {
        Prop = inventorySlotViewModel?.Prop;

        UpdateProp();
    }
}
