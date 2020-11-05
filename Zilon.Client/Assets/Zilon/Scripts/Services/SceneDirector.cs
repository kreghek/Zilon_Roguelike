using System.Linq;

using Assets.Zilon.Scripts.Services;

using JetBrains.Annotations;

using UnityEngine;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Persons;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.ActorInteractionEvents;

public sealed class SceneDirector : MonoBehaviour
{
    [NotNull] [Inject] private readonly ISectorUiState _sectorUiState;

    [NotNull] [Inject] private readonly IActorInteractionBus _actorInteractionBus;

    [NotNull] [Inject] private readonly ILogService _logService;

    [NotNull] [Inject] private readonly UiSettingService _uiSettingService;

    public SectorVM SectorViewModel;
    public DamageIndicator DamageIndicatorPrefab;
    public BlockIndicator BlockIndicatorPrefab;
    public PureMissIndicator PureMissIndicatorPrefab;
    public DodgeIndicator DodgeIndicatorPrefab;
    public BloodTracker BloodTrackerPrefab;
    public BlockSparks BlockSparksPrefab;

    public void Start()
    {
        _actorInteractionBus.NewEvent += ActorInteractionBus_NewEvent;
    }

    public void OnDestroy()
    {
        _actorInteractionBus.NewEvent -= ActorInteractionBus_NewEvent;
    }

    private void ActorInteractionBus_NewEvent(object sender, NewActorInteractionEventArgs e)
    {
        var currentLanguage = _uiSettingService.CurrentLanguage;
        switch (e.ActorInteractionEvent)
        {
            case DamageActorInteractionEvent interactionEvent:
                var damageLog = GetDamageLog(currentLanguage, interactionEvent);

                _logService.Log(damageLog);

                if (interactionEvent.DamageEfficientCalcResult.TargetSuccessfullUsedArmor)
                {
                    var usedArmorLog = $"{interactionEvent.TargetActor} successfully used armor rank: {interactionEvent.DamageEfficientCalcResult.ArmorRank}, roll: {interactionEvent.DamageEfficientCalcResult.FactArmorSaveRoll}, success: {interactionEvent.DamageEfficientCalcResult.SuccessArmorSaveRoll}.";
                    _logService.Log(usedArmorLog);
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

    private string GetDamageLog(Language currentLanguage, DamageActorInteractionEvent interactionEvent)
    {
        string damageTemplate;
        MonsterPerson logPerson;
        if (interactionEvent.Actor == _sectorUiState.ActiveActor.Actor)
        {
            damageTemplate = StaticPhrases.GetValue("log-player-damage-template", currentLanguage);
            logPerson = interactionEvent.TargetActor.Person as MonsterPerson;
        }
        else
        {
            damageTemplate = StaticPhrases.GetValue("log-monster-damage-template", currentLanguage);
            logPerson = interactionEvent.Actor.Person as MonsterPerson;
        }

        var logActorDisplayName = LocalizationHelper.GetValueOrDefaultNoname(currentLanguage, logPerson.Scheme.Name);
        var damageLog = string.Format(
                damageTemplate,
                logActorDisplayName,
                interactionEvent.DamageEfficientCalcResult.ResultEfficient);
        return damageLog;
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
            // Пока индикаторы уроне не выводим. См #678.
            // Сейчас индикаторы вносят больше путаницы.
            //CreateNumericDamageIndicator(interactionEvent, damagedActorViewModel);

            CreateBloodTracker(damagedActorViewModel);
        }
        else
        {
            CreateNoDamageIndicator(damagedActorViewModel, BlockIndicatorPrefab);

            var attackActorViewModel = SectorViewModel.ActorViewModels.SingleOrDefault(x => x.Actor == interactionEvent.Actor);
            if (attackActorViewModel != null)
            {
                CreateBlockSparks(damagedActorViewModel, attackActorViewModel);
            }
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
        bloodTracker.Init(SectorViewModel.transform, damagedActorViewModel);
    }

    private void CreateBlockSparks(ActorViewModel damagedActorViewModel, ActorViewModel attackerActorViewModel)
    {
        var blockSparks = Instantiate(BlockSparksPrefab);
        blockSparks.transform.SetParent(SectorViewModel.transform);

        var targetPosition = damagedActorViewModel.transform.position;
        var attackerPosition = attackerActorViewModel.transform.position;

        blockSparks.transform.position = targetPosition;
        // Искры летят в сторону атакующего
        blockSparks.transform.LookAt(attackerPosition);
    }

    private void CreateNumericDamageIndicator(DamageActorInteractionEvent interactionEvent, ActorViewModel damagedActorViewModel)
    {
        var damageIndicator = Instantiate(DamageIndicatorPrefab);
        damageIndicator.CurrentLanguage = _uiSettingService.CurrentLanguage;
        damageIndicator.transform.SetParent(SectorViewModel.transform);
        damageIndicator.Init(damagedActorViewModel, interactionEvent.DamageEfficientCalcResult.ResultEfficient);
    }

    private void CreateNoDamageIndicator(ActorViewModel actorViewModel, NoDamageIndicatorBase missIndicatorPrefab)
    {
        var indicator = Instantiate(missIndicatorPrefab);
        indicator.CurrentLanguage = _uiSettingService.CurrentLanguage;
        indicator.transform.SetParent(SectorViewModel.transform);
        indicator.Init(actorViewModel);
    }
}

