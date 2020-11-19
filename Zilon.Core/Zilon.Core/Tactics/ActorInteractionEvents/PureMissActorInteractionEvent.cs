namespace Zilon.Core.Tactics.ActorInteractionEvents
{
    public class PureMissActorInteractionEvent : ActorInteractionEventBase
    {
        public PureMissActorInteractionEvent(IActor actor, IActor targetActor) : base(actor)
        {
            TargetActor = targetActor ?? throw new System.ArgumentNullException(nameof(targetActor));
        }

        public int FactToHitRoll { get; internal set; }

        public int SuccessToHitRoll { get; internal set; }

        public IActor TargetActor { get; }
    }
}