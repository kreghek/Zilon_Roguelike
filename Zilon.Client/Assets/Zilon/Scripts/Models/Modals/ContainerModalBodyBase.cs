using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using UnityEngine;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Props;

namespace Assets.Zilon.Scripts.Models.Modals
{
    public class ContainerModalBodyBase : MonoBehaviour
    {
        public PropInfoPopup PropInfoPopup;

        [NotNull] public PropItemVm PropItemPrefab;

        [NotNull] public Transform ContainerItemsParent;

        [NotNull] [Inject] private readonly ICommandManager _clientCommandExecutor;

        [NotNull] [Inject(Id = "prop-transfer-command")] private readonly ICommand _propTransferCommand;

        [NotNull] protected List<PropItemVm> ContainerViewModels { get; private set; }

        [NotNull] protected PropTransferMachine TransferMachine { get; private set; }

        public void Init(PropTransferMachine transferMachine)
        {
            ContainerViewModels = new List<PropItemVm>();

            TransferMachine = transferMachine;

            ((PropTransferCommand)_propTransferCommand).TransferMachine = transferMachine;

            UpdateProps();
            TakeAll();
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

        // Метод виртуальный, а не абстрактный, потому что в Unity нельзя иметь абстрактные монобехи.
        protected virtual void UpdateProps() { }

        private void PropItemViewModel_MouseExit(object sender, EventArgs e)
        {
            PropInfoPopup.SetPropViewModel(null);
        }

        private void PropItemViewModel_MouseEnter(object sender, EventArgs e)
        {
            var currentItemVm = (PropItemVm)sender;
            PropInfoPopup.SetPropViewModel(currentItemVm);
        }

        protected void UpdatePropsInner(Transform itemsParent,
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

        protected void InventoryPropItem_Click(object sender, EventArgs e)
        {
            var currentItemViewModel = (PropItemVm)sender;
            TransferMachine.TransferProp(currentItemViewModel.Prop,
                PropTransferMachineStore.Inventory,
                PropTransferMachineStore.Container);
        }

        protected void ContainerPropItem_Click(object sender, EventArgs e)
        {
            var currentItemViewModel = (PropItemVm)sender;
            TransferMachine.TransferProp(currentItemViewModel.Prop,
                PropTransferMachineStore.Container,
                PropTransferMachineStore.Inventory);
        }

        private void TakeAll()
        {
            var props = TransferMachine.Container.CalcActualItems();
            foreach (var prop in props)
            {
                TransferMachine.TransferProp(prop,
                    PropTransferMachineStore.Container,
                    PropTransferMachineStore.Inventory);
            }

            _clientCommandExecutor.Push(_propTransferCommand);
        }
    }
}
