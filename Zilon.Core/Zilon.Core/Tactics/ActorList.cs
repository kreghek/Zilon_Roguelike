using System.Collections.Generic;

namespace Zilon.Core.Tactics
{
    public class ActorList : IActorManager
    {
        private readonly List<IActor> _items;

        public IEnumerable<IActor> Actors => _items;

        public ActorList()
        {
            _items = new List<IActor>();
        }

        public void Add(IActor actor)
        {
            _items.Add(actor);
        }
    }
}
