using System;
using System.Collections.Generic;

using Assets.Zilon.Scripts.Models.Modals;

using JetBrains.Annotations;

using UnityEngine;

using Zilon.Core.Client;

public class ContainerPopup : ContainerModalBodyBase
{
    private List<PropItemVm> _containerViewModels;

    // ReSharper disable NotNullMemberIsNotInitialized
    // ReSharper disable UnassignedField.Global
    // ReSharper disable MemberCanBePrivate.Global
#pragma warning disable 649

    // ReSharper restore MemberCanBePrivate.Global

    [NotNull] private PropTransferMachine _transferMachine;

    protected override void UpdateProps()
    {
        var containerItems = _transferMachine.Container.CalcActualItems();

        UpdatePropsInner(ContainerItemsParent, containerItems, ContainerPropItem_Click, _containerViewModels);
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
