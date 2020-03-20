using System.Linq;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.WorldGeneration;

namespace Zilon.Core.World.AgentCards
{
    /// <summary>
    /// Карта, в результате которой один агент входит в оппозицию другому агенту.
    /// </summary>
    /// <remarks>
    /// Оппозиция выражается снятием одного очка ХП другого агента.
    /// Если ХП агента падает до 0, то он выходит из игрового мира.
    /// </remarks>
    public sealed class AgentOpposition : IAgentCard
    {
        public int PowerCost { get; }

        public bool CanUse(Agent agent, Globe globe)
        {
            return true;
        }

        public void Use(Agent agent, Globe globe, IDice dice)
        {
            var availableTargets = globe.Agents.Where(x => x != agent && x.Hp > 0).ToArray();
            if (availableTargets.Any())
            {
                var agentRollIndex = dice.Roll(0, availableTargets.Count() - 1);
                var targetAgent = availableTargets[agentRollIndex];
                targetAgent.Hp--;

                if (targetAgent.Hp <= 0)
                {
                    globe.Agents.Remove(targetAgent);
                    CacheHelper.RemoveAgentFromCell(globe.AgentCells, agent.Location, agent);
                }
            }
        }
    }
}
