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
        /// <param name="sourceStore"> Хранилище-источник. </param>
        /// <param name="distStore"> Хранилище-назначение. </param>
        public void TransferProp(IProp prop, IPropStore sourceStore, IPropStore distStore)
        {
            sourceStore.Remove(prop);
            distStore.Add(prop);
        }
    }
}
