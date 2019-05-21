using System.Linq;

using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Bot.Players.Logics
{
    public sealed class ExitLogicState : LogicStateBase
    {
        private readonly ISectorMap _map;

        private MoveTask _moveTask;

        public ExitLogicState(ISectorManager sectorManager)
        {
            _map = sectorManager.CurrentSector.Map;
        }

        public override IActorTask GetTask(IActor actor, ILogicStrategyData strategyData)
        {
            if (!strategyData.ExitNodes.Any())
            {
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

        private MoveTask CreateMoveTask(IActor actor, IMapNode nearbyExitNode)
        {
            if (!_map.IsPositionAvailableFor(nearbyExitNode, actor))
            {
                return null;
            }

            var moveTask = new MoveTask(actor, nearbyExitNode, _map);

            return moveTask;
        }

        protected override void ResetData()
        {
            _moveTask = null;
        }
    }
}
