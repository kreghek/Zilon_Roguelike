using System;
using System.Linq;

using Assets.Zilon.Scripts;
using Assets.Zilon.Scripts.Models.Modals;

using UnityEngine.UI;

// ReSharper disable once ClassNeverInstantiated.Global
// ReSharper disable once CheckNamespace
public class ContainerModalBody : ContainerModalBodyBase, IModalWindowHandler
{
    // ReSharper disable NotNullMemberIsNotInitialized
    // ReSharper disable UnassignedField.Global
    // ReSharper disable MemberCanBePrivate.Global
#pragma warning disable 649
    public Text InfoText;

    // ReSharper restore MemberCanBePrivate.Global

    public event EventHandler Closed;

    public string Caption => "Loot";

#pragma warning restore 649
    // ReSharper restore UnassignedField.Global
    // ReSharper restore NotNullMemberIsNotInitialized

    protected override void UpdateProps()
    {
        var containerItems = TransferMachine.Container.CalcActualItems();

        if (containerItems.Any())
        {
            InfoText.text = "You found:";
        }
        else
        {
            InfoText.text = "You found: Nothing";
        }


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

    public void CloseWindows()
    {
        Closed?.Invoke(this, new EventArgs());
    }

    public void ApplyChanges()
    {
        
    }

    public void CancelChanges()
    {
        throw new NotImplementedException();
    }
}
