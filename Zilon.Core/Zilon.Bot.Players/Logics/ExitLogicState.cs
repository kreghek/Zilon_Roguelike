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
        private readonly ISector _sector;
        private readonly ISectorMap _map;

        private MoveTask _moveTask;

        public ExitLogicState(ISectorManager sectorManager)
        {
            if (sectorManager is null)
            {
                throw new System.ArgumentNullException(nameof(sectorManager));
            }

            _sector = sectorManager.CurrentSector;
            _map = sectorManager.CurrentSector.Map;
        }

        public override IActorTask GetTask(IActor actor, ILogicStrategyData strategyData)
        {
            if (!strategyData.ExitNodes.Any())
            {
                Complete = true;
                return null;
            }

            var actorNode = actor.Node;
            if (_map.Transitions.TryGetValue(actorNode, out var currentTransition))
            {
                _sector.UseTransition(currentTransition);
                Complete = true;
                return null;
            }

            if (_moveTask == null || _moveTask.IsComplete || !_moveTask.CanExecute())
            {
                var nearbyExitNode = strategyData.ExitNodes
                    .OrderBy(x => _map.DistanceBetween(actor.Node, x))
                    .First();

                _moveTask = CreateMoveTask(actor, nearbyExitNode);

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

        private MoveTask CreateMoveTask(IActor actor, IGraphNode targetExitNode)
        {
            var targetNodeIsBlockedByObstacles = GetObstableInNode(_sector, targetExitNode);
            Debug.Assert(!targetNodeIsBlockedByObstacles,
                "Узел с выходом не должен быть препятствием.");

            if (!_map.IsPositionAvailableFor(targetExitNode, actor))
            {
                return null;
            }

            var moveTask = new MoveTask(actor, targetExitNode, _map);

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