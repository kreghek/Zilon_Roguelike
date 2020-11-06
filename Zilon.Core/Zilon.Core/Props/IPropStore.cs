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
        /// Добавление предмета в хранилище.
        /// </summary>
        /// <param name="prop"> Целевой предмет. </param>
        void Add(IProp prop);

        /// <summary>
        /// Проверяет, что предмет есть в хранилище.
        /// </summary>
        /// <param name="prop">Указанные предмет.</param>
        /// <returns>true - если предмет есть.</returns>
        bool Has(IProp prop);

        /// <summary>
        /// Удаление предмета из хранилища.
        /// </summary>
        /// <param name="prop"> Целевой предмет. </param>
        void Remove(IProp prop);

        /// <summary>
        /// Событие выстреливает, когда в хранилище появляется новый предмет.
        /// </summary>
        /// <remarks>
        /// Это событие не срабатывает, если изменилось количество ресурсов.
        /// </remarks>
        event EventHandler<PropStoreEventArgs> Added;

        /// <summary>
        /// Событие выстреливает, если какой-либо предмет удалён из хранилища.
        /// </summary>
        event EventHandler<PropStoreEventArgs> Removed;

        /// <summary>
        /// Событие выстреливает, когда один из предметов в хранилище изменяется.
        /// </summary>
        /// <remarks>
        /// Используется, когда изменяется количество ресурсов в стаке.
        /// </remarks>
        event EventHandler<PropStoreEventArgs> Changed;
    }
}