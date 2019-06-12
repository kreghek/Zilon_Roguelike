using System.Linq;

using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.WorldGeneration.AgentCards
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

            return highestBranchs.Any() && agent.Hp < 1;
        }

        public string Use(Agent agent, Globe globe, IDice dice)
        {
            var highestBranchs = agent.Skills.OrderBy(x => x.Value)
                                    .Where(x => x.Value >= 1);

            if (highestBranchs.Any())
            {
                var firstBranch = highestBranchs.First();

                var agentDisciple = new Agent
                {
                    Name = agent.Name + " disciple",
                    Location = agent.Location,
                    Realm = agent.Realm,
                    Hp = 3
                };

                agentDisciple.Skills.Add(firstBranch.Key, firstBranch.Value / 2);

                globe.Agents.Add(agent);

                Helper.AddAgentToCell(globe.AgentCells, agentDisciple.Location, agent);

                return $"{agent} has a follower. Meeting {agentDisciple}. His branch is: {firstBranch.Key}.";
            }
            else
            {
                globe.AgentCrisys++;

                return null;
            }
        }
    }
}
