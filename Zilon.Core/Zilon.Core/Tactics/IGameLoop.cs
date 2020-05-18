using System;
using System.Collections.Generic;

using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Tactics
{
    public interface IGameLoop
    {
        IEnumerable<IActor> Update();

        IActorTaskSource[] ActorTaskSources { get; set; }

        event EventHandler Updated;
    }
}