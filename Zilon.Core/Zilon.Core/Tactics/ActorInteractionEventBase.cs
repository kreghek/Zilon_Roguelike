using System;
using System.Diagnostics.CodeAnalysis;

namespace Zilon.Core.Tactics
{
    [ExcludeFromCodeCoverage]
    public abstract class ActorInteractionEventBase : IActorInteractionEvent
    {
        protected ActorInteractionEventBase(IActor actor)
        {
            Actor = actor ?? throw new ArgumentNullException(nameof(actor));
        }

        public IActor Actor { get; }
    }
}