using Zilon.Core.Commands;
using Zilon.Core.Tactics.Behaviour;

namespace Assets.Zilon.Scripts.Models.Globe
{
    public class TaskSourceCollector : IActorTaskSourceCollector
    {
        private readonly IActorTaskSource[] _actorTaskSources;

        public TaskSourceCollector(params IActorTaskSource[] actorTaskSources)
        {
            _actorTaskSources = actorTaskSources;
        }

        public IActorTaskSource[] GetCurrentTaskSources()
        {
            return _actorTaskSources;
        }
    }
}
