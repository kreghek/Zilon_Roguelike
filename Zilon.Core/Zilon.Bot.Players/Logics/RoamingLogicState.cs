using System;
using System.Linq;

using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Bot.Players.Logics
{
    public sealed class RoamingLogicState : LogicStateBase
    {
        private MoveTask _moveTask;
        private IdleTask _idleTask;

        private readonly IDecisionSource _decisionSource;

        public RoamingLogicState(IDecisionSource decisionSource)
        {
            _decisionSource = decisionSource ?? throw new ArgumentNullException(nameof(decisionSource));
        }

        protected override void ResetData()
        {
            _moveTask = null;
            _idleTask = null;
        }

        private MoveTask CreateBypassMoveTask(IActor actor, SectorSnapshot sectorSnapshot)
        {
            var map = sectorSnapshot.Sector.Map;
            var availableNodes = map.Nodes.Where(x => map.DistanceBetween(x, actor.Node) < 5);

            var availableNodesArray = availableNodes as HexNode[] ?? availableNodes.ToArray();
            for (var i = 0; i < 3; i++)
            {
                var targetNode = _decisionSource.SelectTargetRoamingNode(availableNodesArray);

                if (map.IsPositionAvailableFor(targetNode, actor))
                {
                    var moveTask = new MoveTask(actor, targetNode, map);

                    return moveTask;
                }
            }

            return null;
        }

        public override IActorTask GetTask(IActor actor, SectorSnapshot sectorSnapshot, ILogicStrategyData strategyData)
        {
            if (_moveTask == null)
            {
                _moveTask = CreateBypassMoveTask(actor, sectorSnapshot);

                if (_moveTask != null)
                {
                    return _moveTask;
                }
                else
                {
                    // Это может произойти, если актёр не выбрал следующий узел.
                    // Тогда переводим актёра в режим ожидания.

                    _idleTask = new IdleTask(actor, _decisionSource);
                    return _idleTask;
                }
            }
            else
            {
                if (!_moveTask.IsComplete)
                {
                    // Если команда на перемещение к целевой точке патруля не закончена,
                    // тогда продолжаем её.
                    // Предварительно проверяем, не мешает ли что-либо её продолжить выполнять.
                    if (!_moveTask.CanExecute())
                    {
                        _moveTask = CreateBypassMoveTask(actor, sectorSnapshot);
                    }

                    if (_moveTask != null)
                    {
                        return _moveTask;
                    }

                    _idleTask = new IdleTask(actor, _decisionSource);
                    return _idleTask;
                }
                else
                {
                    Complete = true;
                    return null;
                }
            }
        }
    }
}
