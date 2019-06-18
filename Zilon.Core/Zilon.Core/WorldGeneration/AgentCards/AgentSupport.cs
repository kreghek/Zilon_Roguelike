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
            var availableTargets = GetAvailableAgents(agent, globe.Agents);
            if (availableTargets.Any())
            {
                var agentRollIndex = dice.Roll(0, availableTargets.Count() - 1);
                var targetAgent = availableTargets[agentRollIndex];
                targetAgent.Hp++;
            }
        }

        private List<Agent> GetAvailableAgents(Agent currentAgent, IEnumerable<Agent> agents)
        {
            var result = new List<Agent>();

            foreach (var agent in agents)
            {
                if (agent != currentAgent)
                {
                    continue;
                }

                if (0 <= agent.Hp && agent.Hp <= 2)
                {
                    result.Add(agent);
                }
            }

            return result;
        }
    }
}
