using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Zilon.Core.Graphs;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Bot.Players.Logics
{
    //TODO Перепроверить работу этого состояния.
    // Есть подозрение, что оно не работает.
    public sealed class ExploreLogicState : LogicStateBase
    {
        private MoveTask _moveTask;
        private IdleTask _idleTask;

        private readonly IDecisionSource _decisionSource;
        private readonly ISectorMap _map;

        public ExploreLogicState(IDecisionSource decisionSource, ISectorManager sectorManager)
        {
            _decisionSource = decisionSource ?? throw new ArgumentNullException(nameof(decisionSource));
            _map = sectorManager.CurrentSector.Map;
        }

        protected override void ResetData()
        {
            _moveTask = null;
            _idleTask = null;
        }

        public override IActorTask GetTask(IActor actor, ILogicStrategyData strategyData)
        {
            if (_moveTask == null)
            {
                _moveTask = CreateBypassMoveTask(actor, strategyData);

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
                        _moveTask = CreateBypassMoveTask(actor, strategyData);
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

        private IEnumerable<IGraphNode> WriteObservedNodes(IActor actor, ILogicStrategyData strategyData)
        {
            var observeNodes = _map.Nodes.Where(x => _map.DistanceBetween(x, actor.Node) < 5);

            foreach (var mapNode in observeNodes)
            {
                strategyData.ObserverdNodes.Add(mapNode);
            }

            // Собираем пограничные неисследованные узлы.
            var frontNodes = new HashSet<IGraphNode>();
            foreach (var observedNode in strategyData.ObserverdNodes)
            {
                var nextNodes = _map.GetNext(observedNode);

                var notObservedNextNodes = nextNodes.Where(x => !strategyData.ObserverdNodes.Contains(x));

                foreach (var edgeNode in notObservedNextNodes)
                {
                    frontNodes.Add(edgeNode);
                }

                // Примечаем выходы
                if (_map.Transitions.ContainsKey(observedNode))
                {
                    strategyData.ExitNodes.Add(observedNode);
                }
            }

            var emptyFrontNodes = !frontNodes.Any();
            var allNodesObserved = _map.Nodes.All(x => strategyData.ObserverdNodes.Contains(x));

            Debug.Assert((emptyFrontNodes && allNodesObserved) || !emptyFrontNodes,
                "Это состояние выполняется, только если есть неисследованые узлы.");

            return frontNodes;
        }

        private MoveTask CreateBypassMoveTask(IActor actor, ILogicStrategyData strategyData)
        {
            IEnumerable<IGraphNode> availableNodes;
            var frontNodes = WriteObservedNodes(actor, strategyData).ToArray();
            if (frontNodes.Any())
            {
                availableNodes = frontNodes;
            }
            else
            {
                availableNodes = strategyData.ObserverdNodes;
            }

            var availableNodesArray = availableNodes as HexNode[] ?? availableNodes.ToArray();
            for (var i = 0; i < 3; i++)
            {
                var targetNode = _decisionSource.SelectTargetRoamingNode(availableNodesArray);

                if (_map.IsPositionAvailableFor(targetNode, actor))
                {
                    var moveTask = new MoveTask(actor, targetNode, _map);

                    return moveTask;
                }
            }

            return null;
        }
    }
}
