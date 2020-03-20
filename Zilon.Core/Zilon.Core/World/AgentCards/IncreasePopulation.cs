using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.World.AgentCards
{
    /// <summary>
    /// Карта увеличения популяции в населённом пункте.
    /// </summary>
    /// <remarks>
    /// Популяция увеличивается, если в текущем нас.пункте меньше 5 ед. популяции.
    /// </remarks>
    public class IncreasePopulation : IAgentCard
    {
        public int PowerCost { get; }

        public bool CanUse(Agent agent, Globe globe)
        {
            if (globe.LocalitiesCells.TryGetValue(agent.Location, out var currentLocality))
            {
                return currentLocality.Population <= 5;
            }
            else
            {
                return false;
            }
        }

        public void Use(Agent agent, Globe globe, IDice dice)
        {
            if (globe.LocalitiesCells.TryGetValue(agent.Location, out var currentLocality))
            {
                currentLocality.Population++;
            }
        }
    }
}
