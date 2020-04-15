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
            if (actor is null)
            {
                throw new System.ArgumentNullException(nameof(actor));
            }

            if (currentState is null)
            {
                throw new System.ArgumentNullException(nameof(currentState));
            }

            if (strategyData is null)
            {
                throw new System.ArgumentNullException(nameof(strategyData));
            }

            var containers = _sectorManager.CurrentSector.StaticObjectManager.Items.Where(x => x.HasModule<IPropContainer>());
            var foundContainers = LootHelper.FindAvailableContainers(containers,
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
