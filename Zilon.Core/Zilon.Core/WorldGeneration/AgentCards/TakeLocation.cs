using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zilon.Core.Common;
using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.WorldGeneration.AgentCards
{
    class TakeLocation : IAgentCard
    {
        public int PowerCost { get; }

        public bool CanUse(Agent agent, Globe globe)
        {
            var targetLocality = GetNeighborLocality(agent, globe);

            return targetLocality != null;
        }

        public void Use(Agent agent, Globe globe, IDice dice)
        {
            var targetLocality = GetNeighborLocality(agent, globe);

            targetLocality.Population--;

            targetLocality.Owner = agent.Realm;

            if (globe.AgentCells.TryGetValue(targetLocality.Cell, out var otherAgentsInLocality))
            {
                foreach (var otherAgent in otherAgentsInLocality.ToArray())
                {
                    TransferAgent(otherAgent, globe, agent.Realm, dice);
                }
            }

            agent.Localtion = targetLocality.Cell;
        }

        private static Locality GetNeighborLocality(Agent agent, Globe globe)
        {
            Locality targetLocality = null;

            var nextCoords = HexHelper.GetOffsetClockwise().OrderBy(item => Guid.NewGuid()).ToArray();
            var agentCubeCoords = HexHelper.ConvertToCube(agent.Localtion.Coords.X, agent.Localtion.Coords.Y);
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
                    targetLocality = otherCheckLocality;
                    break;
                }
            }

            return targetLocality;
        }

        private void TransferAgent(Agent agent, Globe globe, Realm realm, IDice dice)
        {
            globe.LocalitiesCells.TryGetValue(agent.Localtion, out var currentLocality);

            var realmLocalities = globe.Localities.Where(x => x.Owner == agent.Realm && currentLocality != x).ToArray();
            if (!realmLocalities.Any())
            {
                agent.Hp -= 2;
                agent.Realm = realm;
            }

            var rolledTransportLocalityIndex = dice.Roll(0, realmLocalities.Length - 1);
            var rolledTransportLocality = realmLocalities[rolledTransportLocalityIndex];

            if (currentLocality != null)
            {
                Helper.RemoveAgentToCell(globe.AgentCells, agent.Localtion, agent);
            }

            agent.Localtion = rolledTransportLocality.Cell;

            Helper.AddAgentToCell(globe.AgentCells, agent.Localtion, agent);
        }
    }
}
