using System;

namespace Zilon.Core.Tactics
{
    public sealed class ActorInteractionBus : IActorInteractionBus
    {
        public void PushEvent(IActorInteractionEvent interactionEvent)
        {
            var eventArgs = new NewActorInteractionEventArgs(interactionEvent);
            NewEvent?.Invoke(this, eventArgs);
        }

        public event EventHandler<NewActorInteractionEventArgs>? NewEvent;
    }
}