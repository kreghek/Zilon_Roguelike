using System.Linq;

using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players.Triggers
{
    public sealed class ExploredTrigger : ILogicStateTrigger
    {
        public void Reset()
        {
            // Нет состояния.
        }

        public bool Test(
            IActor actor,
            ISectorTaskSourceContext context,
            ILogicState currentState,
            ILogicStrategyData strategyData)
        {
            if (context is null)
            {
                throw new System.ArgumentNullException(nameof(context));
            }

            var allNodesObserved = context.Sector.Map.Nodes.All(x => strategyData.ObserverdNodes.Contains(x));
            return allNodesObserved;
        }

        public void Update()
        {
            // Нет состояния.
        }
    }
}