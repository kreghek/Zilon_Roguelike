using System.Collections.Generic;
using System.Linq;
using Zilon.Core.Common;
using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.WorldGeneration.AgentCards
{
    public class CreateLocality : IAgentCard
    {
        public int PowerCost { get; }

        public bool CanUse(Agent agent, Globe globe)
        {
            var hasCurrentLocality = globe.localitiesCells.TryGetValue(agent.Localtion, out var currentLocality);
            if (currentLocality != null)
            {
                if (currentLocality.Population >= 2)
                {
                    return true;
                }
            }

            return false;
        }

        public void Use(Agent agent, Globe globe, IDice dice)
        {
            globe.localitiesCells.TryGetValue(agent.Localtion, out var currentLocality);

            var highestBranchs = agent.Skills.OrderBy(x => x.Value)
                                    .Where(x => /*x.Key != BranchType.Politics &&*/ x.Value >= 1);
            if (highestBranchs.Any())
            {
                var firstBranch = highestBranchs.First();

                TerrainCell freeLocaltion = null;

                var nextCoords = HexHelper.GetOffsetClockwise();
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

                    if (!globe.localitiesCells.TryGetValue(freeLocaltion1, out var freeCheckLocality))
                    {
                        freeLocaltion = globe.Terrain[freeX][freeY];
                    }
                }

                if (freeLocaltion != null)
                {
                    var createdLocality = new Locality
                    {
                        Name = currentLocality.Name + " " + agent.Name,
                        Branches = new Dictionary<BranchType, int> { { firstBranch.Key, 1 } },
                        Cell = freeLocaltion,

                        Population = 1,
                        Owner = currentLocality.Owner
                    };

                    currentLocality.Population--;

                    globe.localities.Add(createdLocality);
                    globe.localitiesCells[freeLocaltion] = createdLocality;
                    globe.ScanResult.Free.Remove(freeLocaltion);
                }
                else
                {
                    var realmLocalities = globe.localities.Where(x => x.Owner == agent.Realm).ToArray();
                    var rolledTransportLocalityIndex = dice.Roll(0, realmLocalities.Length - 1);
                    var rolledTransportLocality = realmLocalities[rolledTransportLocalityIndex];

                    Helper.RemoveAgentToCell(globe.agentCells, agent.Localtion, agent);

                    agent.Localtion = rolledTransportLocality.Cell;

                    Helper.AddAgentToCell(globe.agentCells, agent.Localtion, agent);
                }
            }
        }
    }
}
