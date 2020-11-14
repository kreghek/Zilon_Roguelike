using System;

namespace Zilon.Core.Tactics
{
    public sealed class NewActorInteractionEventArgs : EventArgs
    {
        public NewActorInteractionEventArgs(IActorInteractionEvent actorInteractionEvent)
        {
            ActorInteractionEvent =
                actorInteractionEvent ?? throw new ArgumentNullException(nameof(actorInteractionEvent));
        }

        public IActorInteractionEvent ActorInteractionEvent { get; }
    }
}