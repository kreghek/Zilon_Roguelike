using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.WorldGeneration.AgentCards
{
    public class IncreasePopulation : IAgentCard
    {
        public int PowerCost { get; }

        public bool CanUse(Agent agent, Globe globe)
        {
            globe.LocalitiesCells.TryGetValue(agent.Localtion, out var currentLocality);

            return currentLocality.Population <= 5;
        }

        public void Use(Agent agent, Globe globe, IDice dice)
        {
            globe.LocalitiesCells.TryGetValue(agent.Localtion, out var currentLocality);
            currentLocality.Population++;
        }
    }
}
