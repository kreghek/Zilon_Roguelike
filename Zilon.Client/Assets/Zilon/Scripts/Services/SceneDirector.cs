using System.Linq;

using Assets.Zilon.Scripts.Services;

using JetBrains.Annotations;

using UnityEngine;

using Zenject;

using Zilon.Core.Tactics;
using Zilon.Core.Tactics.ActorInteractionEvents;


public sealed class SceneDirector : MonoBehaviour
{
    [NotNull] [Inject] private readonly IActorInteractionBus _actorInteractionBus;

    [NotNull] [Inject] private readonly ILogService _logService;

    public SectorVM SectorViewModel;
    public DamageIndicator DamageIndicatorPrefab;

    public void Start()
    {
        _actorInteractionBus.NewEvent += ActorInteractionBus_NewEvent;
    }

    private void ActorInteractionBus_NewEvent(object sender, NewActorInteractionEventArgs e)
    {
        switch (e.ActorInteractionEvent)
        {
            case DamageActorInteractionEvent interactionEvent:
                _logService.Log($"{interactionEvent.Actor} damage {interactionEvent.TargetActor} on {interactionEvent.DamageEfficientCalcResult.ResultEfficient}");

                if (interactionEvent.DamageEfficientCalcResult.TargetSuccessfullUsedArmor)
                {
                    _logService.Log($"{interactionEvent.TargetActor} successfully used armor rank: {interactionEvent.DamageEfficientCalcResult.ArmorRank}, roll: {interactionEvent.DamageEfficientCalcResult.FactArmorSaveRoll}, success: {interactionEvent.DamageEfficientCalcResult.SuccessArmorSaveRoll}.");
                }

                var damagedActorViewModel = SectorViewModel.ActorViewModels.SingleOrDefault(x => x.Actor == interactionEvent.TargetActor);
                if (damagedActorViewModel != null)
                {
                    var damageIndicator = Instantiate(DamageIndicatorPrefab);
                    damageIndicator.transform.SetParent(SectorViewModel.transform.parent);
                    damageIndicator.Init(damagedActorViewModel, interactionEvent.DamageEfficientCalcResult.ResultEfficient);
                }
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

