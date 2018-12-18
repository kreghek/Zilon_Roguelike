using System;
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

            var selectedActorViewModel = GetSelectedActorViewModel();
            if (selectedActorViewModel == null)
            {
                return false;
            }

            var targetNode = selectedActorViewModel.Actor.Node;

            var act = PlayerState.ActiveActor.Actor.Person.TacticalActCarrier.Acts.FirstOrDefault();
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

            if (act.Constrains?.PropResourceType != null)
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
                                where prop is Resource
                                where prop.Scheme.Bullet?.Caliber == usedPropResourceType
                                select prop;

            if (propResources.FirstOrDefault() is Resource propResource)
            {
                if (propResource.Count >= usedPropResourceCount)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private IActorViewModel GetSelectedActorViewModel()
        {
            return PlayerState.HoverViewModel as IActorViewModel;
        }

        protected override void ExecuteTacticCommand()
        {
            var targetActorViewModel = (IActorViewModel)PlayerState.HoverViewModel;

            var targetActor = targetActorViewModel.Actor;
            var intention = new Intention<AttackTask>(a => new AttackTask(a, targetActor, _tacticalActUsageService));
            PlayerState.TaskSource.Intent(intention);
        }
    }
}