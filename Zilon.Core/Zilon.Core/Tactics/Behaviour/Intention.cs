namespace Zilon.Core.Tactics.Behaviour
{
    public sealed class Intention<TActorTask> : IIntention where TActorTask : IActorTask
    {
        public Intention(Func<IActor, TActorTask> taskFactory)
        {
            TaskFactory = taskFactory;
        }

        public Func<IActor, TActorTask> TaskFactory { get; }

        public IActorTask CreateActorTask(IActor actor)
        {
            var task = TaskFactory(actor);
            return task;
        }
    }
}