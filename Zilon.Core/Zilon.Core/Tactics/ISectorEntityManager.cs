using System;
using System.Collections.Generic;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Общий интерфейс для всех менеджеров сущностей сектора
    /// </summary>
    /// <typeparam name="TSectorEntity">
    /// Тип сущности сектора.
    /// Сейчас это либо <see cref="IActor">IActor</see> либо <see cref="IPropContainer">IPropContainer</see>.
    /// </typeparam>
    public interface ISectorEntityManager<TSectorEntity> where TSectorEntity: class
    {
        /// <summary>
        /// Текущий список всех актёров.
        /// </summary>
        IEnumerable<TSectorEntity> Items { get; }

        /// <summary>
        /// Добавляет сущность в общий список.
        /// </summary>
        /// <param name="entity"> Целевая сущность. </param>
        void Add(TSectorEntity entity);

        /// <summary>
        /// Добавляет несколько сущностей в общикй список.
        /// </summary>
        /// <param name="entities"> Набор целевых сущностей. </param>
        void Add(IEnumerable<TSectorEntity> entities);

        /// <summary>
        /// Удаляет актёра из общего списка.
        /// </summary>
        /// <param name="entity"> Целевая сущность. </param>
        void Remove(TSectorEntity entity);

        /// <summary>
        /// Удаляет актёра из общего списка.
        /// </summary>
        /// <param name="entities"> Набор целевых сущностей. </param>
        void Remove(IEnumerable<TSectorEntity> entities);

        /// <summary>
        /// Событие выстреливает, когда в менеджере добавляются сущности.
        /// </summary>
        event EventHandler<ManagerItemsChangedEventArgs<TSectorEntity>> Added;

        /// <summary>
        /// Событие выстреливает, когда из менеджера удаляются сущности.
        /// </summary>
        event EventHandler<ManagerItemsChangedEventArgs<TSectorEntity>> Removed;
    }
}
