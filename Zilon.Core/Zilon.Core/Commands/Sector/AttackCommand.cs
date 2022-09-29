using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Zilon.Core.Client;
using Zilon.Core.Components;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.StaticObjectModules;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Commands
{
    /// <inheritdoc />
    /// <summary>
    /// Команда на перемещение взвода в указанный узел карты.
    /// </summary>
    public class AttackCommand : ActorCommandBase
    {
        private readonly IPlayer _player;
        private readonly ITacticalActUsageService _tacticalActUsageService;

        [ExcludeFromCodeCoverage]
        public AttackCommand(
            IPlayer player,
            ISectorUiState playerState,
            ITacticalActUsageService tacticalActUsageService) :
            base(playerState)
        {
            _player = player;
            _tacticalActUsageService = tacticalActUsageService;
        }

        public override CanExecuteCheckResult CanExecute()
        {
            var sector = _player.SectorNode.Sector;
            if (sector is null)
            {
                throw new InvalidOperationException();
            }

            var map = sector.Map;

            var activeActorViewModel = PlayerState.ActiveActor;
            if (activeActorViewModel is null)
            {
                return CanExecuteCheckResult.CreateFailed("Active actor is not assigned.");
            }

            var currentNode = activeActorViewModel.Actor.Node;

            var target = GetTarget(PlayerState);
            if (target is null)
            {
                return CanExecuteCheckResult.CreateFailed("Invalid target.");
            }

            var targetNode = target.Node;

            var act = PlayerState.TacticalAct;
            if (act is null)
            {
                return CanExecuteCheckResult.CreateFailed("Act is not assigned.");
            }

            if (act.Constrains?.EnergyCost > 0 && activeActorViewModel.Actor.Person.GetModule<ISurvivalModule>().Stats
                    .SingleOrDefault(x => x.Type == SurvivalStatType.Energy)?.Value < 0)
            {
                return CanExecuteCheckResult.CreateFailed("The energy is critically low.");
            }

            if ((act.Stats.Targets & TacticalActTargets.Self) > 0 &&
                ReferenceEquals(activeActorViewModel.Actor, target))
            {
                // Лечить можно только самого себя.
                // Возможно, дальше будут компаньоны и другие НПЦ.
                // Тогда эту проверку нужно будет доработать.
                return CanExecuteCheckResult.CreateSuccessful();
            }

            // Проверка, что цель достаточно близка по дистации и видна.
            if (act.Stats.Range == null)
            {
                return CanExecuteCheckResult.CreateFailed("Act range is not valid.");
            }

            var isInDistance = act.CheckDistance(currentNode, targetNode, sector.Map);
            if (!isInDistance)
            {
                return CanExecuteCheckResult.CreateFailed("Target is out of range.");
            }

            var targetIsOnLine = map.TargetIsOnLine(currentNode, targetNode);
            if (!targetIsOnLine)
            {
                return CanExecuteCheckResult.CreateFailed("Target is not on line of sight.");
            }

            // Проверка наличия ресурсов для выполнения действия

            if (act.Constrains?.PropResourceType != null && act.Constrains?.PropResourceCount != null)
            {
                var hasPropResource = CheckPropResource(
                    activeActorViewModel.Actor.Person.GetModule<IInventoryModule>(),
                    act.Constrains.PropResourceType,
                    act.Constrains.PropResourceCount.Value);

                if (!hasPropResource)
                {
                    return CanExecuteCheckResult.CreateFailed("Has not enought resources to perform act.");
                }
            }

            // Проверка КД

            if (act.CurrentCooldown > 0)
            {
                return CanExecuteCheckResult.CreateFailed("Act cooldown is not over.");
            }

            return CanExecuteCheckResult.CreateSuccessful();
        }

        protected override void ExecuteTacticCommand()
        {
            var target = GetTarget(PlayerState);
            if (target is null)
            {
                throw new InvalidOperationException();
            }

            var tacticalAct = PlayerState.TacticalAct;
            if (tacticalAct is null)
            {
                throw new InvalidOperationException();
            }

            var sector = _player.SectorNode.Sector;
            if (sector is null)
            {
                throw new InvalidOperationException();
            }

            var taskContext = new ActorTaskContext(sector);

            var intention = new Intention<AttackTask>(a =>
                new AttackTask(a, taskContext, target, tacticalAct, _tacticalActUsageService));
            var actor = PlayerState.ActiveActor?.Actor;
            if (actor is null)
            {
                throw new InvalidOperationException();
            }

            var taskSource = PlayerState.TaskSource;
            if (taskSource is null)
            {
                throw new InvalidOperationException();
            }

            taskSource.Intent(intention, actor);
        }

        private static void AddResourceOfUsageToList(
            string usedPropResourceType,
            int requiredCount,
            IList<Resource> propResources,
            Resource propResource)
        {
            var bulletData = propResource.Scheme.Bullet;
            if (bulletData is null)
            {
                return;
            }

            var isRequiredResourceType = string.Equals(
                bulletData.Caliber,
                usedPropResourceType,
                StringComparison.InvariantCulture);

            if (!isRequiredResourceType)
            {
                return;
            }

            if (propResource.Count < requiredCount)
            {
                return;
            }

            propResources.Add(propResource);
        }

        private static bool CheckPropResource(IPropStore inventory,
            string usedPropResourceType,
            int usedPropResourceCount)
        {
            var props = inventory.CalcActualItems();
            var propResources = new List<Resource>();
            foreach (var prop in props)
            {
                if (prop is Resource propResource)
                {
                    AddResourceOfUsageToList(usedPropResourceType, usedPropResourceCount, propResources, propResource);
                }

                // Остальные типы предметов пока не могут выступать, как источник ресурса.
                // Далее нужно будет сделать, чтобы:
                // 1. У персонажа был предмет экипировки, который позволяет выполнять
                // определённые действия другим предметов. Условно, симбиоз двух предметов (или сет предметов).
                // 2. У персонажа был экипирован предмет, который позволяет выполнять
                // определённые действия другим предметов.
                // 3. Расход прочности другого предмета.
                // 4. Применение обойм. Механика расхода предметов, когда ресурсы изымаются не из инвентаря,
                // а их специального контейнера внутри предмета. При необходимости, предмет нужно перезаряжать за
                // отдельное время.
            }

            var preferredPropResource = propResources.FirstOrDefault();

            return preferredPropResource != null && preferredPropResource.Count >= usedPropResourceCount;
        }

        private static IActorViewModel? GetCanExecuteActorViewModel(ISectorUiState sectorUiState)
        {
            var hover = sectorUiState.HoverViewModel as IActorViewModel;
            var selected = sectorUiState.SelectedViewModel as IActorViewModel;
            return hover ?? selected;
        }

        private static IContainerViewModel? GetCanExecuteStaticObjectViewModel(ISectorUiState sectorUiState)
        {
            var hover = sectorUiState.HoverViewModel as IContainerViewModel;
            var selected = sectorUiState.SelectedViewModel as IContainerViewModel;
            return hover ?? selected;
        }

        private static IAttackTarget? GetTarget(ISectorUiState sectorUiState)
        {
            var selectedActorViewModel = GetCanExecuteActorViewModel(sectorUiState);
            var selectedStaticObjectViewModel = GetCanExecuteStaticObjectViewModel(sectorUiState);
            var canTakeDamage = selectedStaticObjectViewModel?.StaticObject?.GetModuleSafe<IDurabilityModule>()?.Value >
                                0;
            if (!canTakeDamage)
            {
                selectedStaticObjectViewModel = null;
            }

            return (IAttackTarget?)selectedActorViewModel?.Actor ?? selectedStaticObjectViewModel?.StaticObject;
        }
    }
}