using System;

using Assets.Zilon.Scripts.Models.Modals;

public class ContainerPopup : ContainerModalBodyBase
{
    protected override void UpdateProps()
    {
        var containerItems = TransferMachine.Container.CalcActualItems();

        UpdatePropsInner(ContainerItemsParent, containerItems, ContainerPropItem_Click, ContainerViewModels);
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
}
