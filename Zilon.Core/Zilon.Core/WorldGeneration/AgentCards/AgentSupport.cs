using System.Collections.Generic;
using System.Linq;

using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.WorldGeneration.AgentCards
{
    /// <summary>
    /// Карточка, когда один агент поддерживает другого.
    /// </summary>
    /// <remarks>
    /// Поддержка выражается в увеличение количества ХП целевого агента.
    /// </remarks>
    public sealed class AgentSupport : IAgentCard
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
                targetAgent.Hp++;
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

                if (agent != currentAgent && (0 <= agent.Hp && agent.Hp <= 2))
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
