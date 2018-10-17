using System;

namespace Zilon.Core.Props
{
    /// <summary>
    /// Интерфейс хранилища предметов.
    /// </summary>
    /// <remarks>
    /// Реализуется инвентарём актёра, контейнером с предметами,
    /// временным хранилищем для сброса на пол.
    /// </remarks>
    public interface IPropStore
    {
        /// <summary>
        /// Предметы в инвентаре.
        /// </summary>
        IProp[] CalcActualItems();

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

        event EventHandler<PropStoreEventArgs> Added;
        event EventHandler<PropStoreEventArgs> Removed;
    }
}