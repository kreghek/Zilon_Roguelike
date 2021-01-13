using System;

namespace Zilon.Core.Tactics
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public abstract class ActorInteractionEventBase : IActorInteractionEvent
    {
        protected ActorInteractionEventBase(IActor actor)
        {
            Actor = actor ?? throw new ArgumentNullException(nameof(actor));
        }

        public IActor Actor { get; }
    }
}