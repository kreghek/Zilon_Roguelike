using System.Linq;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.World.NameGeneration;
using Zilon.Core.WorldGeneration;

namespace Zilon.Core.World.AgentCards
{
    /// <summary>
    /// Карта последователя.
    /// </summary>
    /// <remarks>
    /// В результате выполнения этой карты будет создан последователь текущего агента.
    /// Специальность последователя будет совпадать с высшей специальностью текущего агента,
    /// если специальность развита больше минимального значения.
    /// Последователь может быть создан, если агент уже не имеет влияния (какая-то спорная фигня, нужно будет пересмотреть и убрать).
    /// В случае, если агенту не удалось создать ученика, повышается счётчик кризиса агентов.
    /// </remarks>
    public sealed class Disciple : IAgentCard
    {
        public int PowerCost { get; }

        public bool CanUse(Agent agent, Globe globe)
        {
            var highestBranchs = agent.Skills.OrderBy(x => x.Value)
                                    .Where(x => x.Value >= 1);

            var hasSpeciality = highestBranchs.Any();
            var notDead = agent.Hp < 1;
            var agentNotLimitReached = globe.Agents.Count() <= 120;

            return hasSpeciality && notDead && agentNotLimitReached;
        }

        public void Use(Agent agent, Globe globe, IDice dice)
        {
            var highestBranchs = agent.Skills.OrderBy(x => x.Value)
                                    .Where(x => x.Value >= 1);

            var agentNameGenerator = globe.agentNameGenerator;

            if (highestBranchs.Any())
            {
                var firstBranch = highestBranchs.First();

                var agentDisciple = new Agent
                {
                    Name = agentNameGenerator.Generate(Sex.Male, 1),
                    Location = agent.Location,
                    Realm = agent.Realm,
                    Hp = 3
                };

                agentDisciple.Skills.Add(firstBranch.Key, firstBranch.Value / 2);

                globe.Agents.Add(agent);

                CacheHelper.AddAgentToCell(globe.AgentCells, agentDisciple.Location, agent);
            }
            else
            {
                globe.AgentCrisys++;
            }
        }
    }
}
