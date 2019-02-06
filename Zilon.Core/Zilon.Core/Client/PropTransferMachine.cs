﻿using System;

using Zilon.Core.Props;

namespace Zilon.Core.Client
{
    /// <summary>
    /// Машина состояний трансфера предметов.
    /// </summary>
    /// <remarks>
    /// Используется для трансфтера предметов между инвентарём и контейнером.
    /// </remarks>
    public sealed class PropTransferMachine
    {
        /// <summary>
        /// Конструктор машины состояний.
        /// </summary>
        /// <param name="inventory"> Инвентарь персонажа. </param>
        /// <param name="container"> Контейнер, между которым будет произведён трансфер. </param>
        public PropTransferMachine(IPropStore inventory, IPropStore container)
        {
            Inventory = new PropTransferStore(inventory);
            Container = new PropTransferStore(container);
        }

        /// <summary>
        /// Состояние трансфера предметов в инвентаре персонажа.
        /// </summary>
        public PropTransferStore Inventory { get; }

        /// <summary>
        /// Состояние трансфера предметов в контейнере.
        /// </summary>
        public PropTransferStore Container { get; }

        /// <summary>
        /// Перенос предмета между указанными хранилищами.
        /// </summary>
        /// <param name="prop"> Предмет, который будет перенесён. </param>
        /// <param name="sourceStoreType"> Хранилище-источник. </param>
        /// <param name="distStoreType"> Хранилище-назначение. </param>
        public void TransferProp(IProp prop,
            PropTransferMachineStores sourceStoreType,
            PropTransferMachineStores distStoreType)
        {
            var sourceStore = GetStore(sourceStoreType);
            sourceStore.Remove(prop);

            var distStore = GetStore(distStoreType);
            distStore.Add(prop);
        }

        private IPropStore GetStore(PropTransferMachineStores transferStoreType)
        {
            switch (transferStoreType)
            {
                case PropTransferMachineStores.Inventory:
                    return Inventory;

                case PropTransferMachineStores.Container:
                    return Container;

                default:
                    throw new ArgumentException($"Неизвестный тип контейнера для трансфера {transferStoreType}");
            }
        }
    }
}
