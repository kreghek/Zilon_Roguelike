using Zilon.Core.Tactics.Behaviour;

namespace Assets.Zilon.Scripts.Models.Globe
{
    public class TaskSourceCollector : IActorTaskSourceCollector
    {
        private readonly IActorTaskSource<ISectorTaskSourceContext>[] _actorTaskSources;

        public TaskSourceCollector(params IActorTaskSource<ISectorTaskSourceContext>[] actorTaskSources)
        {
            _actorTaskSources = actorTaskSources;
        }

        public IActorTaskSource<ISectorTaskSourceContext>[] GetCurrentTaskSources()
        {
            return _actorTaskSources;
        }
    }
}
