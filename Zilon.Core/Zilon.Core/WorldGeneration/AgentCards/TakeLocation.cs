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
            var targetLocality = GetNeighborLocality(agent, globe, 0);

            return targetLocality != null;
        }

        public void Use(Agent agent, Globe globe, IDice dice)
        {
            var targetLocality = GetNeighborLocality(agent, globe, dice.Roll(0, 5));

            targetLocality.Population--;

            targetLocality.Owner = agent.Realm;

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

            agent.Location = targetLocality.Cell;
        }

        private static Locality GetNeighborLocality(Agent agent, Globe globe, int coordRollIndex)
        {
            Locality targetLocality = null;
            CubeCoords[] nextCoords = GetRandomCoords(coordRollIndex);
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

        private static CubeCoords[] GetRandomCoords(int coordRollIndex)
        {
            var coords = HexHelper.GetOffsetClockwise();
            var shuffledCoords = new CubeCoords[6];
            for (var i = 0; i < 6; i++)
            {
                var coordRollIndexOffset = (coordRollIndex + 1) % 6;
                shuffledCoords[coordRollIndexOffset] = coords[i];
            }

            return shuffledCoords;
        }

        private void TransferAgent(Agent agent, Globe globe, Realm realm, IDice dice)
        {
            globe.LocalitiesCells.TryGetValue(agent.Location, out var currentLocality);

            var wasTransported = TransportHelper.TransportAgentToRandomLocality(globe, dice, agent, currentLocality);

            if (!wasTransported)
            {
                // Агент переходит в государство-захватчик.
                // При этом получает урон.
                agent.Hp -= 2;
                agent.Realm = realm;
            }
        }
    }
}
