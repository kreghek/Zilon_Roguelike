using System.Collections.Generic;

namespace Zilon.Core.Tactics
{
    public interface IActorList
    {
        IEnumerable<IActor> Actors { get; }
    }
}
