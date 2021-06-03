using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Persons;

namespace Zilon.Core.Tactics.ActorInteractionEvents
{
    public record ActDescription
    {
        public ActDescription(IEnumerable<string> tags)
        {
            Tags = tags ?? throw new ArgumentNullException(nameof(tags));
        }

        public IEnumerable<string> Tags { get; }
    }

    [ExcludeFromCodeCoverage]
    public sealed class DamageActorInteractionEvent : ActorInteractionEventBase
    {
        public DamageActorInteractionEvent(IActor actor, IActor targetActor, ActDescription usedActDescription, DamageEfficientCalc damageEfficientCalcResult) : base(actor)
        {
            TargetActor = targetActor ?? throw new ArgumentNullException(nameof(targetActor));
            UsedActDescription = usedActDescription;
            DamageEfficientCalcResult = damageEfficientCalcResult;
        }

        public DamageEfficientCalc DamageEfficientCalcResult { get; }
        public int FactToHitRoll { get; internal set; }
        public int SuccessToHitRoll { get; internal set; }

        public IActor TargetActor { get; }
        public ActDescription UsedActDescription { get; }
    }
}