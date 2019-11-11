using System.Collections.Generic;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.WorldGeneration.NameGeneration;

namespace Zilon.Core.WorldGeneration.LocalityEventCards
{
    public sealed class NewActivistEvent : ILocalityEventCard
    {
        public bool CanUse(Locality locality, Globe globe)
        {
            if (globe.Agents.Count >= 120)
            {
                return false;
            }

            return true;
        }

        public void Use(Locality locality, Globe globe, IDice dice)
        {
            var agentNameGenerator = globe.agentNameGenerator;

            var agent = new Agent
            {
                Name = agentNameGenerator.Generate(Sex.Male, 1),
                Location = locality.Cell,
                Realm = locality.Owner,
                Hp = 3
            };

            var rolledBranchIndex = dice.Roll(0, 7);
            agent.Skills = new Dictionary<BranchType, int>
                {
                    { (BranchType)rolledBranchIndex, 1 }
                };


            globe.Agents.Add(agent);

            CacheHelper.AddAgentToCell(globe.AgentCells, agent.Location, agent);
        }
    }
}
