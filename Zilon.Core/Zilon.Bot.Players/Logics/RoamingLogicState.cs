using Zilon.Core.Graphs;
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

        private MoveTask CreateBypassMoveTask(IActor actor, ISector sector)
        {
            ISectorMap map = sector.Map;
            var availableNodes = map.Nodes.Where(x => map.DistanceBetween(x, actor.Node) < 5);

            HexNode[] availableNodesArray = availableNodes as HexNode[] ?? availableNodes.ToArray();
            for (var i = 0; i < 3; i++)
            {
                IGraphNode targetNode = DecisionSource.SelectTargetRoamingNode(availableNodesArray);

                if (map.IsPositionAvailableFor(targetNode, actor))
                {
                    ActorTaskContext taskContext = new ActorTaskContext(sector);
                    MoveTask moveTask = new MoveTask(actor, taskContext, targetNode, map);

                    return moveTask;
                }
            }

            return null;
        }

        public override IActorTask GetTask(IActor actor, ISectorTaskSourceContext context,
            ILogicStrategyData strategyData)
        {
            if (MoveTask == null)
            {
                MoveTask = CreateBypassMoveTask(actor, context.Sector);

                if (MoveTask != null)
                {
                    return MoveTask;
                }
                // Это может произойти, если актёр не выбрал следующий узел.
                // Тогда переводим актёра в режим ожидания.

                ActorTaskContext taskContext = new ActorTaskContext(context.Sector);
                IdleTask = new IdleTask(actor, taskContext, DecisionSource);
                return IdleTask;
            }

            if (!MoveTask.IsComplete)
            {
                // Если команда на перемещение к целевой точке патруля не закончена,
                // тогда продолжаем её.
                // Предварительно проверяем, не мешает ли что-либо её продолжить выполнять.
                if (!MoveTask.CanExecute())
                {
                    MoveTask = CreateBypassMoveTask(actor, context.Sector);
                }

                if (MoveTask != null)
                {
                    return MoveTask;
                }

                ActorTaskContext taskContext = new ActorTaskContext(context.Sector);
                IdleTask = new IdleTask(actor, taskContext, DecisionSource);
                return IdleTask;
            }

            Complete = true;
            return null;
        }
    }
}