namespace Zilon.Core.Tactics
{
    public abstract class ActorInteractionEventBase : IActorInteractionEvent
    {
        protected ActorInteractionEventBase(IActor actor)
        {
            Actor = actor ?? throw new ArgumentNullException(nameof(actor));
        }

        public IActor Actor { get; }
    }
}