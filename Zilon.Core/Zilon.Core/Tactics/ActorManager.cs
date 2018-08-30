using System;
using System.Collections.Generic;
using System.Linq;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Менеджер актёров. Берёт на себя всю работу для предоставления
    /// списка текущих актёров в секторе.
    /// </summary>
    public class ActorManager : IActorManager
    {
        private readonly List<IActor> _items;

        public event EventHandler<ManagerItemsChangedArgs<IActor>> Added;

        /// <summary>
        /// Текущий список всех актёров.
        /// </summary>
        public IEnumerable<IActor> Actors => _items;

        public ActorManager()
        {
            _items = new List<IActor>();
        }

        /// <summary>
        /// Добавляет актёра в общий список.
        /// </summary>
        /// <param name="actor"> Целевой актёр. </param>
        public void Add(IActor actor)
        {
            _items.Add(actor);

            DoAdd(actor);
        }

        /// <summary>
        /// Добавляет несколько актёров в общикй список.
        /// </summary>
        /// <param name="actors"> Перечень актёров. </param>
        public void Add(IEnumerable<IActor> actors)
        {
            _items.AddRange(actors);

            DoAdd(actors.ToArray());
        }


        private void DoAdd(params IActor[] actor)
        {
            var args = new ManagerItemsChangedArgs<IActor>(actor);
            Added?.Invoke(this, args);
        }
    }
}
