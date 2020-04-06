using System.Collections.Generic;

using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Tactics
{
    public interface ITaskSourceProvider
    {
        IEnumerable<IActorTaskSource> GetTaskSources();
    }
}
