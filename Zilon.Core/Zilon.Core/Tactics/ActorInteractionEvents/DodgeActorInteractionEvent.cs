using Zilon.Core.Persons;

namespace Zilon.Core.Tactics.ActorInteractionEvents
{
    public class DodgeActorInteractionEvent : ActorInteractionEventBase
    {
        public DodgeActorInteractionEvent(IActor actor, IActor targetActor, PersonDefenceItem personDefenceItem) :
            base(actor)
        {
            TargetActor = targetActor ?? throw new System.ArgumentNullException(nameof(targetActor));
            PersonDefenceItem = personDefenceItem ?? throw new System.ArgumentNullException(nameof(personDefenceItem));
        }

        public int FactToHitRoll { get; internal set; }

        public PersonDefenceItem PersonDefenceItem { get; }

        public int SuccessToHitRoll { get; internal set; }

        public IActor TargetActor { get; }
    }
}