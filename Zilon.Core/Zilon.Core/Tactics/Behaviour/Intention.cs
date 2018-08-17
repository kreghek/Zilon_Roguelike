using System;

namespace Zilon.Core.Tactics.Behaviour
{
    public sealed class Intention<TActorTask> : IIntention where TActorTask : IActorTask
    {
        private readonly Func<IActor, TActorTask> _taskFactory;

        public Intention(Func<IActor, TActorTask> taskFactory)
        {
            _taskFactory = taskFactory;
        }

        public IActorTask CreateActorTask(IActorTask currentTask, IActor actor)
        {
            var task = _taskFactory(actor);
            return task;
        }
    }
}
