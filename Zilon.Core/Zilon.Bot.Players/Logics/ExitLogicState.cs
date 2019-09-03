using System.Diagnostics;
using System.Linq;

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

        private MoveTask CreateMoveTask(IActor actor, IMapNode targetExitNode)
        {
            Debug.Assert((targetExitNode as HexNode)?.IsObstacle != true,
                "Узел с выходом не должен быть препятствием.");

            if (!_map.IsPositionAvailableFor(targetExitNode, actor))
            {
                return null;
            }

            var moveTask = new MoveTask(actor, targetExitNode, _map);

            return moveTask;
        }

        protected override void ResetData()
        {
            _moveTask = null;
        }
    }
}
