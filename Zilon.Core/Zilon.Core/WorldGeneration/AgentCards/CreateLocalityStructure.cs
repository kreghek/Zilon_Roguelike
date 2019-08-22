using System;

using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.WorldGeneration.AgentCards
{
    class CreateLocalityStructure : IAgentCard
    {
        public int PowerCost { get; }

        public bool CanUse(Agent agent, Globe globe)
        {
            var currentAgentCell = agent.Location;
            var currentAgentLocality = globe.LocalitiesCells[currentAgentCell];

            var manufacture = currentAgentLocality.Stats.Resources[LocalityResource.Manufacture];

            return manufacture > 5; // Условно, столько производства требуется для всех зданий.
        }

        public void Use(Agent agent, Globe globe, IDice dice)
        {
            throw new NotImplementedException();
        }
    }
}
