using System.Linq;

using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Bot.Players.Triggers
{
    public sealed class ExploredTrigger : ILogicStateTrigger
    {
        private readonly ISectorMap _map;

        public ExploredTrigger(ISectorManager sectorManager)
        {
            _map = sectorManager.CurrentSector.Map;
        }

        public void Reset()
        {
            // Нет состояния.
        }

        public bool Test(IActor actor, ILogicState currentState, ILogicStrategyData strategyData)
        {
            var allNodesObserved = _map.Nodes.All(x => strategyData.ObserverdNodes.Contains(x));
            return allNodesObserved;
        }

        public void Update()
        {
            // Нет состояния.
        }
    }
}