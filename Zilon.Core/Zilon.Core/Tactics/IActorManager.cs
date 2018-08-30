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

        event EventHandler<ManagerItemsChangedArgs<IActor>> Added;
    }
}
