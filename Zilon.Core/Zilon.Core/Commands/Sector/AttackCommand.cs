using System.Collections.Generic;
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
        public AttackCommand(
            ITacticalActUsageService tacticalActUsageService) :
            base()
        {
            _tacticalActUsageService = tacticalActUsageService;
        }

        public override bool CanExecute(SectorCommandContext context)
        {
            var map = context.CurrentSector.Map;

            var currentNode = context.ActiveActor.Actor.Node;

            var selectedActorViewModel = GetCanExecuteActorViewModel(context);
            if (selectedActorViewModel == null)
            {
                return false;
            }

            var targetNode = selectedActorViewModel.Actor.Node;

            var act = context.ActiveActor.Actor.Person.TacticalActCarrier.Acts.First();
            if ((act.Stats.Targets & TacticalActTargets.Self) > 0 &&
                context.ActiveActor.Actor == selectedActorViewModel.Actor)
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
                    var hasPropResource = CheckPropResource(context.ActiveActor.Actor.Person.Inventory,
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

        private IActorViewModel GetCanExecuteActorViewModel(SectorCommandContext context)
        {
            var hover = context.HoverViewModel as IActorViewModel;
            var selected = context.SelectedViewModel as IActorViewModel;
            return hover ?? selected;
        }

        protected override void ExecuteTacticCommand(SectorCommandContext context)
        {
            var targetActorViewModel = (IActorViewModel)context.SelectedViewModel;

            var targetActor = targetActorViewModel.Actor;
            var intention = new Intention<AttackTask>(a => new AttackTask(a, targetActor, _tacticalActUsageService));
            context.TaskSource.Intent(intention);
        }
    }
}