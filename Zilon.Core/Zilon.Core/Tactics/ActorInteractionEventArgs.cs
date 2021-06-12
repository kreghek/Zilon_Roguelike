using System;

namespace Zilon.Core.Tactics
{
    public sealed class ActorInteractionEventArgs : EventArgs
    {
        public ActorInteractionEventArgs(IActorInteractionEvent actorInteractionEvent)
        {
            ActorInteractionEvent =
                actorInteractionEvent ?? throw new ArgumentNullException(nameof(actorInteractionEvent));
        }

        public IActorInteractionEvent ActorInteractionEvent { get; }
    }
}