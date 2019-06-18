using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Common;
using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.WorldGeneration.AgentCards
{
    /// <summary>
    /// Карта позволяет забрать соседний нас.пункт под контроль государста агента.
    /// </summary>
    /// <remarks>
    /// 1. Все агенты, находящиеся в нас.пункте, будут отправлены в произвольные нас.пункты своего государства.
    /// 2. У всех агентов будет снято 2 единицы влияния.
    /// 3. Популяция текущего нас.пункта уменьшается на 1.
    /// </remarks>
    public sealed class TakeLocation : IAgentCard
    {
        public int PowerCost { get; }

        public bool CanUse(Agent agent, Globe globe)
        {
            var targetLocality = GetNeighborLocality(agent, globe);

            return targetLocality != null;
        }

        public string Use(Agent agent, Globe globe, IDice dice)
        {
            var targetLocality = GetNeighborLocality(agent, globe);

            targetLocality.Population--;

            targetLocality.Owner = agent.Realm;

            var history = $"{agent} conquered {targetLocality} for his realm {agent.Realm}.";

            var otherAgentHistory = new List<string>();
            if (globe.AgentCells.TryGetValue(targetLocality.Cell, out var otherAgentsInLocality))
            {
                foreach (var otherAgent in otherAgentsInLocality.ToArray())
                {
                    if (otherAgent.Realm == agent.Realm)
                    {
                        continue;
                    }

                    TransferAgent(otherAgent, globe, agent.Realm, dice);
                    otherAgentHistory.Add($"{otherAgent} in {otherAgent.Location}.");
                }
            }

            history += " " + string.Join(" ", otherAgentHistory);

            agent.Location = targetLocality.Cell;

            return history;
        }

        private static Locality GetNeighborLocality(Agent agent, Globe globe)
        {
            Locality targetLocality = null;

            var nextCoords = HexHelper.GetOffsetClockwise().OrderBy(item => Guid.NewGuid()).ToArray();
            var agentCubeCoords = HexHelper.ConvertToCube(agent.Location.Coords.X, agent.Location.Coords.Y);
            for (var i = 0; i < nextCoords.Length; i++)
            {
                var scanCubeCoords = agentCubeCoords + nextCoords[i];
                var scanOffsetCoords = HexHelper.ConvertToOffset(scanCubeCoords);

                var freeX = scanOffsetCoords.X;
                var freeY = scanOffsetCoords.Y;

                if (freeX < 0)
                {
                    continue;
                }

                if (freeX >= globe.Terrain.Length)
                {
                    continue;
                }

                if (freeY < 0)
                {
                    continue;
                }

                if (freeY >= globe.Terrain[freeX].Length)
                {
                    continue;
                }

                var freeLocaltion1 = globe.Terrain[freeX][freeY];

                if (globe.LocalitiesCells.TryGetValue(freeLocaltion1, out var otherCheckLocality))
                {
                    // Захватываем только города, которые не принадлежат нашему государству.
                    if (otherCheckLocality.Owner != agent.Realm)
                    {
                        targetLocality = otherCheckLocality;
                        break;
                    }
                }
            }

            return targetLocality;
        }

        private void TransferAgent(Agent agent, Globe globe, Realm realm, IDice dice)
        {
            globe.LocalitiesCells.TryGetValue(agent.Location, out var currentLocality);

            var rolledTransportLocality = TransportHelper.RollTargetLocality(globe, dice, agent, currentLocality);

            if (rolledTransportLocality == null)
            {
                // Агент переходит в государство-захватчик.
                // При этом получает урон.
                agent.Hp -= 2;
                agent.Realm = realm;
            }
            else
            {
                if (currentLocality != null)
                {
                    Helper.RemoveAgentFromCell(globe.AgentCells, agent.Location, agent);
                }

                agent.Location = rolledTransportLocality.Cell;

                Helper.AddAgentToCell(globe.AgentCells, agent.Location, agent);
            }
        }
    }
}
