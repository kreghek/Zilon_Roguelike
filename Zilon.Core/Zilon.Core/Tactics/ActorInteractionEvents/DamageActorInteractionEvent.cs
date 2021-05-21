using System;
using System.Diagnostics.CodeAnalysis;

namespace Zilon.Core.Tactics.ActorInteractionEvents
{
    [ExcludeFromCodeCoverage]
    public sealed class DamageActorInteractionEvent : ActorInteractionEventBase
    {
        public DamageActorInteractionEvent(IActor actor, IActor targetActor,
            DamageEfficientCalc damageEfficientCalcResult) : base(actor)
        {
            TargetActor = targetActor ?? throw new ArgumentNullException(nameof(targetActor));
            DamageEfficientCalcResult = damageEfficientCalcResult;
        }

        public DamageEfficientCalc DamageEfficientCalcResult { get; }
        public int FactToHitRoll { get; internal set; }
        public int SuccessToHitRoll { get; internal set; }

        public IActor TargetActor { get; }
    }
}