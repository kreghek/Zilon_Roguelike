using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Zilon.Core.Client;
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
            IPlayerState playerState,
            ITacticalActUsageService tacticalActUsageService) :
            base(gameLoop, sectorManager, playerState)
        {
            _tacticalActUsageService = tacticalActUsageService;
        }

        public override bool CanExecute()
        {
            var map = SectorManager.CurrentSector.Map;

            var currentNode = PlayerState.ActiveActor.Actor.Node;

            var selectedActorViewModel = GetHoverActorViewModel();
            if (selectedActorViewModel == null)
            {
                return false;
            }

            var targetNode = selectedActorViewModel.Actor.Node;

            var act = PlayerState.ActiveActor.Actor.Person.TacticalActCarrier.Acts.First();
            var isInDistance = act.CheckDistance(((HexNode)currentNode).CubeCoords, ((HexNode)targetNode).CubeCoords);
            if (!isInDistance)
            {
                return false;
            }

            var targetIsOnLine = MapHelper.CheckNodeAvailability(map, currentNode, targetNode);
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


            return true;
        }

        private bool CheckPropResource(IPropStore inventory,
            string usedPropResourceType, 
            int usedPropResourceCount)
        {
            var propResources = from prop in inventory.CalcActualItems()
                                let propResource = prop as Resource
                                where propResource != null
                                where propResource.Scheme.Bullet?.Caliber == usedPropResourceType
                                select propResource;

            var preferredPropResource = propResources.FirstOrDefault();

            return preferredPropResource != null && preferredPropResource.Count >= usedPropResourceCount;
        }

        private IActorViewModel GetHoverActorViewModel()
        {
            return PlayerState.HoverViewModel as IActorViewModel;
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