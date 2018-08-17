namespace Zilon.Core.Tactics.Behaviour
{
    public sealed class OpenContainerIntention : IIntention
    {
        private readonly IPropContainer _container;
        private readonly IOpenContainerMethod _method;

        public OpenContainerIntention(IPropContainer container, IOpenContainerMethod method)
        {
            _container = container;
            _method = method;
        }

        public IActorTask CreateActorTask(IActorTask currentTask, IActor actor)
        {
            var task = new OpenContainerTask(actor, _container, _method);
            return task;
        }
    }
}
