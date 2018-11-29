using System.Linq;

using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour.Bots
{
    /// <summary>
    /// Логика произвольного брожения.
    /// </summary>
    public class RoamingLogic : AgressiveLogicBase
    {
        public RoamingLogic(IActor actor,
            IMap map,
            IActorManager actors,
            IDecisionSource decisionSource,
            ITacticalActUsageService actService): base(actor, map, actors, decisionSource, actService)
        {

        }

        protected override void ProcessMovementComplete()
        {
            // Пока никак не обрабатываем.
            // В конечном счёте сдесь можно сделать учёт уже пройденных узлов комнаты.
        }

        protected override MoveTask CreateBypassMoveTask()
        {
            var currentActorNode = (HexNode)Actor.Node;
            var currentActorCoords = currentActorNode.CubeCoords;
            //TODO Сделать привязку монстра к текущей комнате.
            var avaialbleNodes = Map.Nodes.Cast<HexNode>().Where(x => x.CubeCoords.DistanceTo(currentActorCoords) < 5);
            var targetNode = DecisionSource.SelectTargetRoamingNode(avaialbleNodes);

            var moveTask = new MoveTask(Actor, targetNode, Map);

            return moveTask;
        }

        protected override void ProcessIntruderDetected()
        {
            // Ничего делать не нужно.
        }
    }
}
