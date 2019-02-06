using System.Linq;

using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.WorldGeneration.AgentCards
{
    public sealed class Disciple : IAgentCard
    {
        public int PowerCost { get; }

        public bool CanUse(Agent agent, Globe globe)
        {
            var highestBranchs = agent.Skills.OrderBy(x => x.Value)
                                    .Where(x => x.Value >= 1);

            return highestBranchs.Any() && agent.Hp < 1;
        }

        public void Use(Agent agent, Globe globe, IDice dice)
        {
            var highestBranchs = agent.Skills.OrderBy(x => x.Value)
                                    .Where(x => x.Value >= 1);

            if (highestBranchs.Any())
            {
                var firstBranch = highestBranchs.First();

                var agentDisciple = new Agent
                {
                    Name = agent.Name + " disciple",
                    Localtion = agent.Localtion,
                    Realm = agent.Realm,
                    Hp = 3
                };

                globe.Agents.Add(agent);

                Helper.AddAgentToCell(globe.AgentCells, agentDisciple.Localtion, agent);
            }
            else
            {
                globe.AgentCrisys++;
            }
        }
    }
}
