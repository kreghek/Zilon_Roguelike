using Zilon.Core.Commands;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.GlobeObserver
{
    public class TaskSourceCollector : IActorTaskSourceCollector
    {
        private readonly IActorTaskSource _actorTaskSource;

        public TaskSourceCollector(IActorTaskSource botActorTaskSource)
        {
            _actorTaskSource = botActorTaskSource;
        }

        public IActorTaskSource[] GetCurrentTaskSources()
        {
            return new[] { _actorTaskSource };
        }
    }
}
