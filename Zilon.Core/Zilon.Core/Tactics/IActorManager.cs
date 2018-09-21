using System;
using System.Collections.Generic;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Менеджер актёров. Берёт на себя всю работу для предоставления
    /// списка текущих актёров в секторе.
    /// </summary>
    public interface IActorManager
    {
        /// <summary>
        /// Текущий список всех актёров.
        /// </summary>
        IEnumerable<IActor> Actors { get; }

        /// <summary>
        /// Добавляет актёра в общий список.
        /// </summary>
        /// <param name="actor"> Целевой актёр. </param>
        void Add(IActor actor);

        /// <summary>
        /// Добавляет несколько актёров в общикй список.
        /// </summary>
        /// <param name="actors"> Перечень актёров. </param>
        void Add(IEnumerable<IActor> actors);

        /// <summary>
        /// Удаляет актёра из общего списка.
        /// </summary>
        /// <param name="actor"> Целевой актёр. </param>
        void Remove(IActor actor);

        /// <summary>
        /// Удаляет актёра из общего списка.
        /// </summary>
        /// <param name="actor"> Целевой актёр. </param>
        void Remove(IEnumerable<IActor> actors);

        /// <summary>
        /// Событие выстреливает, когда в менеджере добавляются новые актёры.
        /// </summary>
        event EventHandler<ManagerItemsChangedArgs<IActor>> Added;

        /// <summary>
        /// Событие выстреливает, когда из менеджера удаляются актёры.
        /// </summary>
        event EventHandler<ManagerItemsChangedArgs<IActor>> Removed;
    }
}
