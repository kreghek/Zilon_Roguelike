using System.Collections.Generic;
using System.Linq;

using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.WorldGeneration.AgentCards
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
            var targetAgent = GetTargetAgent(agent, globe.Agents, dice);
            if (targetAgent != null)
            {
                targetAgent.Hp--;

                if (targetAgent.Hp <= 0)
                {
                    globe.Agents.Remove(targetAgent);
                    globe.AgentCells.Remove(targetAgent.Location);
                }
            }
        }

        private Agent GetTargetAgent(Agent currentAgent, List<Agent> agents, IDice dice)
        {
            var agentCount = agents.Count();
            var agentIndex = dice.Roll(0, agentCount - 1);
            var startIndex = agentIndex;
            Agent targetAgent = null;
            while (targetAgent != null)
            {
                var agent = agents[agentIndex];

                if (agent != currentAgent && 0 < agent.Hp)
                {
                    targetAgent = agent;
                }

                agentIndex++;
                if (agentIndex >= agentCount)
                {
                    agentIndex = 0;
                }

                if (startIndex == agentIndex)
                {
                    // Достигли точки, с которой начали обход.
                    // Значит не нашли подходящего агента.

                    break;
                }
            }

            return targetAgent;
        }
    }
}
