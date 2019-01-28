using System.Linq;

using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.WorldGeneration.AgentCards
{
    public sealed class AgentSupport : IAgentCard
    {
        public int PowerCost { get; }

        public bool CanUse(Agent agent, Globe globe)
        {
            return true;
        }

        public void Use(Agent agent, Globe globe, IDice dice)
        {
            var agentRollIndex = dice.Roll(0, globe.Agents.Where(x => x.Hp >= 0).Count() - 1);
            var targetAgent = globe.Agents[agentRollIndex];
            targetAgent.Hp++;
        }
    }
}
