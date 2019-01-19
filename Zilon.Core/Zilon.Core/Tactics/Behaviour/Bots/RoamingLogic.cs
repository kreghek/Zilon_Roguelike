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
            var availableNodes = Map.Nodes.Cast<HexNode>().Where(x => x.CubeCoords.DistanceTo(currentActorCoords) < 5);

            var availableNodesArray = availableNodes as HexNode[] ?? availableNodes.ToArray();
            for (var i = 0; i < 3; i++)
            {
                var targetNode = DecisionSource.SelectTargetRoamingNode(availableNodesArray);

                if (Map.IsPositionAvailableFor(targetNode, Actor))
                {
                    var moveTask = new MoveTask(Actor, targetNode, Map);

                    return moveTask;
                }
            }

            return null;
        }

        protected override void ProcessIntruderDetected()
        {
            // Ничего делать не нужно.
        }
    }
}
