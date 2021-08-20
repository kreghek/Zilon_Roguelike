using System;
using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Persons;

namespace Zilon.Core.Tactics.ActorInteractionEvents
{
    [ExcludeFromCodeCoverage]
    public class PureMissActorInteractionEvent : ActorInteractionEventBase
    {
        public PureMissActorInteractionEvent(IActor actor, IActor targetActor, ActDescription usedActDescription) :
            base(actor)
        {
            TargetActor = targetActor ?? throw new ArgumentNullException(nameof(targetActor));
            UsedActDescription = usedActDescription;
        }

        public int FactToHitRoll { get; internal set; }

        public int SuccessToHitRoll { get; internal set; }

        public IActor TargetActor { get; }
        public ActDescription UsedActDescription { get; }
    }
}