using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Zilon.Core.Client;
using Zilon.Core.Components;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
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
        private readonly ITacticalActUsageService _tacticalActUsageService;

        [ExcludeFromCodeCoverage]
        public AttackCommand(
            ISectorManager sectorManager,
            ISectorUiState playerState,
            ITacticalActUsageService tacticalActUsageService) :
            base(sectorManager, playerState)
        {
            _tacticalActUsageService = tacticalActUsageService;
        }

        public override bool CanExecute()
        {
            var map = SectorManager.CurrentSector.Map;

            var currentNode = PlayerState.ActiveActor.Actor.Node;

            var target = GetTarget(PlayerState);
            if (target is null)
            {
                return false;
            }

            var targetNode = target.Node;

            var act = PlayerState.TacticalAct;
            if ((act.Stats.Targets & TacticalActTargets.Self) > 0 &&
                ReferenceEquals(PlayerState.ActiveActor.Actor, target))
            {
                // Лечить можно только самого себя.
                // Возможно, дальше будут компаньоны и другие НПЦ.
                // Тогда эту проверку нужно будет доработать.
                return true;
            }
            else
            {
                // Проверка, что цель достаточно близка по дистации и видна.
                if (act.Stats.Range == null)
                {
                    return false;
                }

                var isInDistance = act.CheckDistance(currentNode, targetNode, SectorManager.CurrentSector.Map);
                if (!isInDistance)
                {
                    return false;
                }

                var targetIsOnLine = map.TargetIsOnLine(currentNode, targetNode);
                if (!targetIsOnLine)
                {
                    return false;
                }

                // Проверка наличия ресурсов для выполнения действия

                if (act.Constrains?.PropResourceType != null && act.Constrains?.PropResourceCount != null)
                {
                    var hasPropResource = CheckPropResource(PlayerState.ActiveActor.Actor.Person.GetModule<IInventoryModule>(),
                        act.Constrains.PropResourceType,
                        act.Constrains.PropResourceCount.Value);

                    if (!hasPropResource)
                    {
                        return false;
                    }
                }

                // Проверка КД

                if (act.CurrentCooldown > 0)
                {
                    return false;
                }
            }

            return true;
        }

        private static IAttackTarget GetTarget(ISectorUiState sectorUiState)
        {
            var selectedActorViewModel = GetCanExecuteActorViewModel(sectorUiState);
            var selectedStaticObjectViewModel = GetCanExecuteStaticObjectViewModel(sectorUiState);
            var canTakeDamage = selectedStaticObjectViewModel?.StaticObject?.GetModuleSafe<IDurabilityModule>()?.Value > 0;
            if (!canTakeDamage)
            {
                selectedStaticObjectViewModel = null;
            }

            return (IAttackTarget)selectedActorViewModel?.Actor ?? selectedStaticObjectViewModel?.StaticObject;
        }

        protected override void ExecuteTacticCommand()
        {
            var target = GetTarget(PlayerState);

            var tacticalAct = PlayerState.TacticalAct;

            var intention = new Intention<AttackTask>(a => new AttackTask(a, target, tacticalAct, _tacticalActUsageService));
            PlayerState.TaskSource.Intent(intention);
        }

        private bool CheckPropResource(IPropStore inventory,
            string usedPropResourceType,
            int usedPropResourceCount)
        {
            var props = inventory.CalcActualItems();
            var propResources = new List<Resource>();
            foreach (var prop in props)
            {
                var propResource = prop as Resource;
                if (propResource == null)
                {
                    continue;
                }

                if (propResource.Scheme.Bullet?.Caliber == usedPropResourceType)
                {
                    propResources.Add(propResource);
                }
            }

            var preferredPropResource = propResources.FirstOrDefault();

            return preferredPropResource != null && preferredPropResource.Count >= usedPropResourceCount;
        }

        private static IActorViewModel GetCanExecuteActorViewModel(ISectorUiState sectorUiState)
        {
            var hover = sectorUiState.HoverViewModel as IActorViewModel;
            var selected = sectorUiState.SelectedViewModel as IActorViewModel;
            return hover ?? selected;
        }

        private static IContainerViewModel GetCanExecuteStaticObjectViewModel(ISectorUiState sectorUiState)
        {
            var hover = sectorUiState.HoverViewModel as IContainerViewModel;
            var selected = sectorUiState.SelectedViewModel as IContainerViewModel;
            return hover ?? selected;
        }
    }
}