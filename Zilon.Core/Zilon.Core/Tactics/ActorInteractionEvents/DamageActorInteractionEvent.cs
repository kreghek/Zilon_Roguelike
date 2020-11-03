using System;

namespace Zilon.Core.Tactics.ActorInteractionEvents
{
    public sealed class DamageActorInteractionEvent : ActorInteractionEventBase
    {
        public DamageActorInteractionEvent(IActor actor, IActor targetActor, DamageEfficientCalc damageEfficientCalcResult) : base(actor)
        {
            TargetActor = targetActor ?? throw new ArgumentNullException(nameof(targetActor));
            DamageEfficientCalcResult = damageEfficientCalcResult;
        }

        public IActor TargetActor { get; }
        public DamageEfficientCalc DamageEfficientCalcResult { get; }
        public int SuccessToHitRoll { get; internal set; }
        public int FactToHitRoll { get; internal set; }
    }
}