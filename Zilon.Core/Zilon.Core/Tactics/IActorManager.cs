using System.Collections.Generic;

namespace Zilon.Core.Tactics
{
    public interface IActorManager
    {
        IEnumerable<IActor> Actors { get; }

        void Add(IActor actor);
    }
}
