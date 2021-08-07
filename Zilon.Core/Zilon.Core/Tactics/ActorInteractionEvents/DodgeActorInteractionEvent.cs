using System;
using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Persons;

namespace Zilon.Core.Tactics.ActorInteractionEvents
{
    [ExcludeFromCodeCoverage]
    public class DodgeActorInteractionEvent : ActorInteractionEventBase
    {
        public DodgeActorInteractionEvent(IActor actor, IActor targetActor, PersonDefenceItem personDefenceItem, ActDescription usedActDescription) :
            base(actor)
        {
            TargetActor = targetActor ?? throw new ArgumentNullException(nameof(targetActor));
            PersonDefenceItem = personDefenceItem ?? throw new ArgumentNullException(nameof(personDefenceItem));
            UsedActDescription = usedActDescription;
        }

        public int FactToHitRoll { get; internal set; }
        public PersonDefenceItem PersonDefenceItem { get; }
        public ActDescription UsedActDescription { get; }
        public int SuccessToHitRoll { get; internal set; }

        public IActor TargetActor { get; }
    }
}