using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Props;

public class ContainerPopup : MonoBehaviour
{
    private List<PropItemVm> _containerViewModels;

    // ReSharper disable NotNullMemberIsNotInitialized
    // ReSharper disable UnassignedField.Global
    // ReSharper disable MemberCanBePrivate.Global
#pragma warning disable 649
    [NotNull] public PropItemVm PropItemPrefab;

    [NotNull] public Transform ContainerItemsParent;

    public PropInfoPopup PropInfoPopup;

    // ReSharper restore MemberCanBePrivate.Global

    [NotNull] [Inject] private readonly ICommandManager _clientCommandExecutor;

    [NotNull] [Inject(Id = "prop-transfer-command")] private readonly ICommand _propTransferCommand;

    [NotNull] private PropTransferMachine _transferMachine;

    public void Init(PropTransferMachine transferMachine)
    {
        _containerViewModels = new List<PropItemVm>();

        _transferMachine = transferMachine;

        ((PropTransferCommand)_propTransferCommand).TransferMachine = transferMachine;

        UpdateProps();
        TakeAll();
    }

    private void UpdateProps()
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

    private void UpdatePropsInner(Transform itemsParent,
        IEnumerable<IProp> props,
        EventHandler propItemHandler,
        List<PropItemVm> propItems)
    {
        foreach (var prop in props)
        {
            var propItemVm = Instantiate(PropItemPrefab, itemsParent);
            propItemVm.Init(prop);
            propItemVm.Click += propItemHandler;
            propItemVm.MouseEnter += PropItemViewModel_MouseEnter;
            propItemVm.MouseExit += PropItemViewModel_MouseExit;
            propItems.Add(propItemVm);
        }

        var parentRect = itemsParent.GetComponent<RectTransform>();
        var rowCount = (int)Math.Ceiling(props.Count() / 4f);
        parentRect.sizeDelta = new Vector2(parentRect.sizeDelta.x, (40 + 5) * rowCount);
    }

    private void InventoryPropItem_Click(object sender, EventArgs e)
    {
        var currentItemViewModel = (PropItemVm)sender;
        _transferMachine.TransferProp(currentItemViewModel.Prop,
            PropTransferMachineStores.Inventory,
            PropTransferMachineStores.Container);
    }

    private void ContainerPropItem_Click(object sender, EventArgs e)
    {
        var currentItemViewModel = (PropItemVm)sender;
        _transferMachine.TransferProp(currentItemViewModel.Prop,
            PropTransferMachineStores.Container,
            PropTransferMachineStores.Inventory);
    }

    private void TakeAll()
    {
        var props = _transferMachine.Container.CalcActualItems();
        foreach (var prop in props)
        {
            _transferMachine.TransferProp(prop,
                PropTransferMachineStores.Container,
                PropTransferMachineStores.Inventory);
        }

        _clientCommandExecutor.Push(_propTransferCommand);
    }

    public void OnDestroy()
    {
        foreach (Transform propTranfsorm in ContainerItemsParent)
        {
            var propItemViewModel = propTranfsorm.GetComponent<PropItemVm>();
            propItemViewModel.Click -= ContainerPropItem_Click;
            Destroy(propItemViewModel.gameObject);
        }
    }
}
