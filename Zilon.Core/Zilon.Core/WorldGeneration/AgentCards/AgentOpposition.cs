﻿using System.Linq;

using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.WorldGeneration.AgentCards
{
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
                    globe.AgentCells.Remove(targetAgent.Localtion);
                }
            }
        }
    }
}
