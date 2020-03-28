using System.Linq;

using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Bot.Players.Triggers
{
    public sealed class LootDetectedTrigger : ILogicStateTrigger
    {
        private readonly ISectorMap _map;
        private readonly ISectorManager _sectorManager;

        public LootDetectedTrigger(ISectorManager sectorManager)
        {
            _sectorManager = sectorManager ?? throw new System.ArgumentNullException(nameof(sectorManager));

            _map = sectorManager.CurrentSector.Map;
        }

        public void Reset()
        {
            // Нет состояния.
        }

        public bool Test(IActor actor, ILogicState currentState, ILogicStrategyData strategyData)
        {
            var foundContainers = LootHelper.FindAvailableContainers(_sectorManager.CurrentSector.PropContainerManager.Items,
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
