using System.Linq;

using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Bot.Players.Triggers
{
    public sealed class LootDetectedTrigger : ILogicStateTrigger
    {
        private readonly IPropContainerManager _propContainerManager;
        private readonly ISectorMap _map;

        public LootDetectedTrigger(IPropContainerManager propContainerManager, ISectorManager sectorManager)
        {
            _propContainerManager = propContainerManager;
            _map = sectorManager.CurrentSector.Map;
        }

        public void Reset()
        {
            // Нет состояния.
        }

        public bool Test(IActor actor, ILogicState currentState, ILogicStrategyData strategyData)
        {
            var foundContainers = LootHelper.FindAvailableContainers(_propContainerManager.Items,
                actor.Node,
                _map);

            return foundContainers.Any();
        }

        public void Update()
        {
            // Нет состояния.
        }
    }
}
