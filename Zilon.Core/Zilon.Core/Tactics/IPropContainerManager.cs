using System;
using System.Collections.Generic;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Интерфейс менеджера контейнеров предметов.
    /// </summary>
    public interface IPropContainerManager
    {
        /// <summary>
        /// Текущий список всех предметов в секторе.
        /// </summary>
        IEnumerable<IPropContainer> Containers { get; }

        /// <summary>
        /// Добавляет контейнер с предметами в общий список.
        /// </summary>
        /// <param name="propContainer"> Целевой контейнер предметов. </param>
        void Add(IPropContainer propContainer);

        /// <summary>
        /// Добавляет несколько контейнеров предметов в общий список.
        /// </summary>
        /// <param name="propContainers"> Перечень контейнеров предметов. </param>
        void Add(IEnumerable<IPropContainer> propContainers);

        /// <summary>
        /// Удаляет контейнер с предметами из общего списка.
        /// </summary>
        /// <param name="propContainer"> Целевой контейнер предметов. </param>
        void Remove(IPropContainer propContainer);

        /// <summary>
        /// Удаляет несколько контейнеров предметов из общего списка.
        /// </summary>
        /// <param name="propContainers"> Перечень контейнеров предметов. </param>
        void Remove(IEnumerable<IPropContainer> propContainers);

        /// <summary>
        /// Событие выстреливает, когда в менеджере добавляются новые контейнеры.
        /// </summary>
        event EventHandler<ManagerItemsChangedArgs<IPropContainer>> Added;

        /// <summary>
        /// Событие выстреливает, когда из менеджера удаляются контейнеры.
        /// </summary>
        event EventHandler<ManagerItemsChangedArgs<IPropContainer>> Removed;
    }
}
