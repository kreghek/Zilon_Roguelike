using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Commands
{
    public interface IActorTaskSourceCollector
    {
        IActorTaskSource[] GetCurrentTaskSources();
    }
}
