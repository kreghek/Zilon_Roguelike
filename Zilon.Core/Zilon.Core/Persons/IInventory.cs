using System;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Интерфейс инвентаря. Общего или на актёра.
    /// </summary>
    public interface IInventory
    {
        /// <summary>
        /// Предметы в инвентаре.
        /// </summary>
        IProp[] Items { get; }

        /// <summary>
        /// Добавление предмета в инвентарь.
        /// </summary>
        /// <param name="prop"> Целевой предмет. </param>
        void Add(IProp prop);

        /// <summary>
        /// Удаление предмета из инвентаря.
        /// </summary>
        /// <param name="prop"> Целевой предмет. </param>
        void Remove(IProp prop);

        event EventHandler<InventoryEventArgs> Added;
        event EventHandler<InventoryEventArgs> Removed;
        event EventHandler<InventoryEventArgs> Changed;
    }
}