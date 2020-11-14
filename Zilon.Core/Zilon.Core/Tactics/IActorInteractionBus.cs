namespace Zilon.Core.Tactics
{
    public interface IActorInteractionBus
    {
        void PushEvent(IActorInteractionEvent interactionEvent);

        event EventHandler<NewActorInteractionEventArgs> NewEvent;
    }
}