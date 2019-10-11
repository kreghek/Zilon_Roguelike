using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Zilon.Core.Client;
using Zilon.Core.Components;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;

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
        public AttackCommand(IGameLoop gameLoop,
            ISectorManager sectorManager,
            ISectorUiState playerState,
            ITacticalActUsageService tacticalActUsageService) :
            base(gameLoop, sectorManager, playerState)
        {
            _tacticalActUsageService = tacticalActUsageService;
        }

        public override bool CanExecute()
        {
            var map = SectorManager.CurrentSector.Map;

            var currentNode = PlayerState.ActiveActor.Actor.Node;

            var selectedActorViewModel = GetCanExecuteActorViewModel();
            if (selectedActorViewModel == null)
            {
                return false;
            }

            var targetNode = selectedActorViewModel.Actor.Node;

            var act = PlayerState.ActiveActor.Actor.Person.TacticalActCarrier.Acts.First();
            if ((act.Stats.Targets & TacticalActTargets.Self) > 0 &&
                PlayerState.ActiveActor.Actor == selectedActorViewModel.Actor)
            {
                return true;
            }
            else
            {
                if (act.Stats.Range == null)
                {
                    return false;
                }

                var currentCoords = ((HexNode)currentNode).CubeCoords;
                var targetCoords = ((HexNode)targetNode).CubeCoords;
                var isInDistance = act.CheckDistance(currentCoords, targetCoords);
                if (!isInDistance)
                {
                    return false;
                }

                var targetIsOnLine = map.TargetIsOnLine(currentNode, targetNode);
                if (!targetIsOnLine)
                {
                    return false;
                }

                if (act.Constrains?.PropResourceType != null && act.Constrains?.PropResourceCount != null)
                {
                    var hasPropResource = CheckPropResource(PlayerState.ActiveActor.Actor.Person.Inventory,
                        act.Constrains.PropResourceType,
                        act.Constrains.PropResourceCount.Value);

                    if (!hasPropResource)
                    {
                        return false;
                    }
                }
            }


            return true;
        }

        private bool CheckPropResource(IPropStore inventory,
            string usedPropResourceType,
            int usedPropResourceCount)
        {
            var propResources = from prop in inventory.CalcActualItems()
                                let propResource = prop as Resource
                                where propResource != null
                                //TODO propResource проверка не нужна, потому что выше есть propResource != null
                                // Переписать этот блок более очевидно
                                where propResource?.Scheme.Bullet?.Caliber == usedPropResourceType
                                select propResource;

            var preferredPropResource = propResources.FirstOrDefault();

            return preferredPropResource != null && preferredPropResource.Count >= usedPropResourceCount;
        }

        private IActorViewModel GetCanExecuteActorViewModel()
        {
            var hover = PlayerState.HoverViewModel as IActorViewModel;
            var selected = PlayerState.SelectedViewModel as IActorViewModel;
            return hover ?? selected;
        }

        protected override void ExecuteTacticCommand()
        {
            var targetActorViewModel = (IActorViewModel)PlayerState.SelectedViewModel;

            var targetActor = targetActorViewModel.Actor;
            var intention = new Intention<AttackTask>(a => new AttackTask(a, targetActor, _tacticalActUsageService));
            PlayerState.TaskSource.Intent(intention);
        }
    }
}