using System;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

using Zilon.Core.Tactics;
using Zilon.Core.Tactics.ActorInteractionEvents;

namespace Assets.Zilon.Scripts.Services
{
    public sealed class SceneDirector: MonoBehaviour
    {
        [NotNull] [Inject] private readonly IActorInteractionBus _actorInteractionBus;

        [NotNull] [Inject] private readonly ILogService _logService;

        public SectorVM SectorViewModel;
        public DamageIndicator DamageIndicatorPrefab;

        public void Start() {
            _actorInteractionBus.NewEvent += ActorInteractionBus_NewEvent;
        }

        private void ActorInteractionBus_NewEvent(object sender, NewActorInteractionEventArgs e)
        {
            switch (e.ActorInteractionEvent)
            {
                case DamageActorInteractionEvent damageActorInteractionEvent:
                    _logService.Log($"{damageActorInteractionEvent.Actor} damage {damageActorInteractionEvent.TargetActor} on {damageActorInteractionEvent.DamageEfficientCalcResult.ResultEfficient}");

                    if (damageActorInteractionEvent.DamageEfficientCalcResult.TargetSuccessfullUsedArmor)
                    {
                        _logService.Log($"{damageActorInteractionEvent.TargetActor} successfully used armor rank: {damageActorInteractionEvent.DamageEfficientCalcResult.ArmorRank}, roll: {damageActorInteractionEvent.DamageEfficientCalcResult.FactArmorSaveRoll}, success: {damageActorInteractionEvent.DamageEfficientCalcResult.SuccessArmorSaveRoll}.");
                    }

                    var damageIndicator = Instantiate(DamageIndicatorPrefab);
                    damageIndicator.transform.SetParent(transform.parent);
                    damageIndicator.Init(this, e.Value);
                    break;

                case DodgeActorInteractionEvent dodgeActorInteractionEvent:
                    _logService.Log($"{dodgeActorInteractionEvent.Actor} defends {dodgeActorInteractionEvent.PersonDefenceItem}, roll: {dodgeActorInteractionEvent.FactToHitRoll}, success: {dodgeActorInteractionEvent.SuccessToHitRoll}");
                    break;

                case PureMissActorInteractionEvent pureMissActorInteractionEvent:
                    _logService.Log($"{pureMissActorInteractionEvent.Actor} missed.");
                    break;
            }
        }
    }
}
