using System;
using System.Linq;

using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Bot.Players.Logics
{
    public sealed class RoamingLogicState : ILogicState
    {
        private readonly IDecisionSource _decisionSource;
        private readonly ISectorMap _map;

        public RoamingLogicState(IDecisionSource decisionSource, ISectorMap map)
        {
            _decisionSource = decisionSource ?? throw new ArgumentNullException(nameof(decisionSource));
            _map = map;
        }

        public bool Complete { get; }

        public ILogicStateData CreateData(IActor actor)
        {
            //TODO Нужно при создании данных выбирать целевой узел.
            // Логика будет считаться выполненной при достижении или невозможности достижения
            // узла в созданных данных.
            return new RoamingLogicData();
        }

        public IActorTask GetTask(IActor actor, ILogicStateData data)
        {
            var logicData = (RoamingLogicData)data;
            if (logicData.MoveTask == null)
            {
                logicData.MoveTask = CreateBypassMoveTask(actor);

                if (logicData.MoveTask != null)
                {
                    return logicData.MoveTask;
                }
                else
                {
                    // Это может произойти, если актёр не выбрал следующий узел.
                    // Тогда переводим актёра в режим ожидания.

                    logicData.IdleTask = new IdleTask(actor, _decisionSource);
                    return logicData.IdleTask;
                }
            }
            else
            {
                if (!logicData.MoveTask.IsComplete)
                {
                    // Если команда на перемещение к целевой точке патруля не закончена,
                    // тогда продолжаем её.
                    // Предварительно проверяем, не мешает ли что-либо её продолжить выполнять.
                    if (!logicData.MoveTask.CanExecute())
                    {
                        logicData.MoveTask = CreateBypassMoveTask(actor);
                    }

                    if (logicData.MoveTask != null)
                    {
                        return logicData.MoveTask;
                    }

                    logicData.IdleTask = new IdleTask(actor, _decisionSource);
                    return logicData.IdleTask;
                }

                return null;
            }
        }

        private MoveTask CreateBypassMoveTask(IActor actor)
        {
            var availableNodes = _map.Nodes.Where(x => _map.DistanceBetween(x, actor.Node) < 5);

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
