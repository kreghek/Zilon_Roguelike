namespace Zilon.Core.Tactics.Behaviour
{
    /// <summary>
    ///     Базовая реализация сборщика источников команд.
    /// </summary>
    public sealed class ActorTaskSourceCollector : IActorTaskSourceCollector
    {
        private readonly IActorTaskSource<ISectorTaskSourceContext>[] _actorTaskSources;

        public ActorTaskSourceCollector(params IActorTaskSource<ISectorTaskSourceContext>[] actorTaskSources)
        {
            _actorTaskSources = actorTaskSources ?? throw new ArgumentNullException(nameof(actorTaskSources));
        }

        /// <inheritdoc />
        public IActorTaskSource<ISectorTaskSourceContext>[] GetCurrentTaskSources()
        {
            return _actorTaskSources;
        }
    }
}