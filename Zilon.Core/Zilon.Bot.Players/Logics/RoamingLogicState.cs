using System.Linq;

using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Bot.Players.Logics
{
    public sealed class RoamingLogicState : MoveLogicStateBase
    {
        public RoamingLogicState(IDecisionSource decisionSource) : base(decisionSource)
        {
        }

        private MoveTask CreateBypassMoveTask(IActor actor)
        {
            var availableNodes = Map.Nodes.Where(x => Map.DistanceBetween(x, actor.Node) < 5);

            var availableNodesArray = availableNodes as HexNode[] ?? availableNodes.ToArray();
            for (var i = 0; i < 3; i++)
            {
                var targetNode = DecisionSource.SelectTargetRoamingNode(availableNodesArray);

                if (Map.IsPositionAvailableFor(targetNode, actor))
                {
                    var moveTask = new MoveTask(actor, targetNode, Map);

                    return moveTask;
                }
            }

            return null;
        }

        public override IActorTask GetTask(IActor actor, ISectorTaskSourceContext context, ILogicStrategyData strategyData)
        {
            if (MoveTask == null)
            {
                MoveTask = CreateBypassMoveTask(actor);

                if (MoveTask != null)
                {
                    return MoveTask;
                }
                else
                {
                    // Это может произойти, если актёр не выбрал следующий узел.
                    // Тогда переводим актёра в режим ожидания.

                    IdleTask = new IdleTask(actor, DecisionSource);
                    return IdleTask;
                }
            }
            else
            {
                if (!MoveTask.IsComplete)
                {
                    // Если команда на перемещение к целевой точке патруля не закончена,
                    // тогда продолжаем её.
                    // Предварительно проверяем, не мешает ли что-либо её продолжить выполнять.
                    if (!MoveTask.CanExecute())
                    {
                        MoveTask = CreateBypassMoveTask(actor);
                    }

                    if (MoveTask != null)
                    {
                        return MoveTask;
                    }

                    IdleTask = new IdleTask(actor, DecisionSource);
                    return IdleTask;
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
