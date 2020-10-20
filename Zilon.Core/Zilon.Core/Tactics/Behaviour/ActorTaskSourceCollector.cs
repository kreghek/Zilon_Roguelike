using System;

namespace Zilon.Core.Tactics.Behaviour
{
    /// <summary>
    /// Базовая реализация сборщика источников команд.
    /// </summary>
    public sealed class ActorTaskSourceCollector : IActorTaskSourceCollector
    {
        private readonly IActorTaskSource[] _actorTaskSources;

        public ActorTaskSourceCollector(params IActorTaskSource[] actorTaskSources)
        {
            _actorTaskSources = actorTaskSources ?? throw new ArgumentNullException(nameof(actorTaskSources));
        }

        /// <inheritdoc/>
        public IActorTaskSource[] GetCurrentTaskSources()
        {
            return _actorTaskSources;
        }
    }
}