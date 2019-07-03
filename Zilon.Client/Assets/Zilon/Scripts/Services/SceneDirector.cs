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
    public BlockIndicator BlockIndicatorPrefab;
    public PureMissIndicator PureMissIndicatorPrefab;
    public DodgeIndicator DodgeIndicatorPrefab;
    public BloodTracker BloodTrackerPrefab;

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

                CreateDamageIndication(interactionEvent);
                break;

            case DodgeActorInteractionEvent interactionEvent:
                _logService.Log($"{interactionEvent.Actor} defends {interactionEvent.PersonDefenceItem}, roll: {interactionEvent.FactToHitRoll}, success: {interactionEvent.SuccessToHitRoll}");

                CreateDodgeIndication(interactionEvent);
                break;

            case PureMissActorInteractionEvent interactionEvent:
                _logService.Log($"{interactionEvent.Actor} missed.");

                CreateMissIndication(interactionEvent);
                break;
        }
    }

    private void CreateDamageIndication(DamageActorInteractionEvent interactionEvent)
    {
        // Индикатор урона выводим над целевым актёром.
        // Потому что это он получил урон.
        var damagedActorViewModel = SectorViewModel.ActorViewModels.SingleOrDefault(x => x.Actor == interactionEvent.TargetActor);
        if (damagedActorViewModel == null)
        {
            return;
        }

        // Урон выводим, только если он был больше нуля.
        // В ином случае считаем, что цель заблокировала урон.
        if (interactionEvent.DamageEfficientCalcResult.ResultEfficient > 0)
        {
            CreateNumericDamageIndicator(interactionEvent, damagedActorViewModel);
            CreateBloodTracker(damagedActorViewModel);
        }
        else
        {
            CreateNoDamageIndicator(damagedActorViewModel, BlockIndicatorPrefab);
            //TODO Вместо крови выводить трекер блока - белые искры.
        }
    }

    private void CreateMissIndication(PureMissActorInteractionEvent interactionEvent)
    {
        // Индикатор промаха выводим над актёром, который совершал действие.
        // Потому что это он промазал.
        var actorViewModel = SectorViewModel.ActorViewModels.SingleOrDefault(x => x.Actor == interactionEvent.Actor);
        if (actorViewModel == null)
        {
            return;
        }

        CreateNoDamageIndicator(actorViewModel, PureMissIndicatorPrefab);
    }

    private void CreateDodgeIndication(DodgeActorInteractionEvent interactionEvent)
    {
        // Индикатор уклонения выводим над целевым актёром.
        // Потому что это он уклонился.
        var actorViewModel = SectorViewModel.ActorViewModels.SingleOrDefault(x => x.Actor == interactionEvent.TargetActor);
        if (actorViewModel == null)
        {
            return;
        }

        CreateNoDamageIndicator(actorViewModel, DodgeIndicatorPrefab);
    }

    private void CreateBloodTracker(ActorViewModel damagedActorViewModel)
    {
        var bloodTracker = Instantiate(BloodTrackerPrefab);
        bloodTracker.transform.SetParent(SectorViewModel.transform);
        var randomOffset = Random.insideUnitCircle * 0.2f;
        var offsetVector = new Vector3(randomOffset.x, randomOffset.y);
        bloodTracker.transform.position = damagedActorViewModel.transform.position + offsetVector;
        bloodTracker.transform.Rotate(Vector3.up, Random.Range(0, 360));
    }

    private void CreateNumericDamageIndicator(DamageActorInteractionEvent interactionEvent, ActorViewModel damagedActorViewModel)
    {
        var damageIndicator = Instantiate(DamageIndicatorPrefab);
        damageIndicator.transform.SetParent(SectorViewModel.transform);
        damageIndicator.Init(damagedActorViewModel, interactionEvent.DamageEfficientCalcResult.ResultEfficient);
    }

    private void CreateNoDamageIndicator(ActorViewModel actorViewModel, NoDamageIndicatorBase missIndicatorPrefab)
    {
        var indicator = Instantiate(missIndicatorPrefab);
        indicator.transform.SetParent(SectorViewModel.transform);
        indicator.Init(actorViewModel);
    }
}

