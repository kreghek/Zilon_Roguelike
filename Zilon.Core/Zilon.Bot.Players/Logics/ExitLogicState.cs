using System.Diagnostics;
using System.Linq;

using Zilon.Core.Graphs;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Bot.Players.Logics
{
    public sealed class ExitLogicState : LogicStateBase
    {
        private MoveTask _moveTask;

        public override IActorTask GetTask(IActor actor, ISectorTaskSourceContext context, ILogicStrategyData strategyData)
        {
            var sector = context.Sector;
            var map = sector.Map;

            if (!strategyData.ExitNodes.Any())
            {
                Complete = true;
                return null;
            }

            var actorNode = actor.Node;
            if (map.Transitions.TryGetValue(actorNode, out var currentTransition))
            {
                System.Console.WriteLine("trans");
                sector.UseTransition(actor, currentTransition);
                Complete = true;
                return null;
            }

            if (_moveTask == null || _moveTask.IsComplete || !_moveTask.CanExecute())
            {
                var nearbyExitNode = strategyData.ExitNodes
                    .OrderBy(x => map.DistanceBetween(actor.Node, x))
                    .First();

                _moveTask = CreateMoveTask(actor, nearbyExitNode, sector, map);

                if (_moveTask == null)
                {
                    Complete = true;
                    return null;
                }

                return _moveTask;
            }
            else
            {
                return _moveTask;
            }
        }

        private static MoveTask CreateMoveTask(IActor actor, IGraphNode targetExitNode, ISector sector, ISectorMap map)
        {
            var targetNodeIsBlockedByObstacles = GetObstableInNode(sector, targetExitNode);
            Debug.Assert(!targetNodeIsBlockedByObstacles,
                "Узел с выходом не должен быть препятствием.");

            if (!map.IsPositionAvailableFor(targetExitNode, actor))
            {
                return null;
            }

            var context = new ActorTaskContext(sector);

            var moveTask = new MoveTask(actor, context, targetExitNode, map);

            return moveTask;
        }

        private static bool GetObstableInNode(ISector sector, IGraphNode node)
        {
            var staticObstaclesInTargetNode = sector.StaticObjectManager.Items.Where(x => x.Node == node && x.IsMapBlock);
            var targetNodeIsBlockedByObstacles = staticObstaclesInTargetNode.Any();
            return targetNodeIsBlockedByObstacles;
        }

        protected override void ResetData()
        {
            _moveTask = null;
        }
    }
}
