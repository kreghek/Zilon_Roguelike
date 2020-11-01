namespace Zilon.Core.Tactics
{
    public sealed class ActorInteractionBus : IActorInteractionBus
    {
        public event EventHandler<NewActorInteractionEventArgs> NewEvent;

        public void PushEvent(IActorInteractionEvent interactionEvent)
        {
            NewActorInteractionEventArgs eventArgs = new NewActorInteractionEventArgs(interactionEvent);
            NewEvent?.Invoke(this, eventArgs);
        }
    }
}