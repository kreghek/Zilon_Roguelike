using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.WorldGeneration.AgentCards
{
    public class CreateLocality : IAgentCard
    {
        public int PowerCost { get; }

        public bool CanUse(Agent agent, Globe globe)
        {
            return globe.localitiesCells.TryGetValue(agent.Localtion, out var currentLocality) &&
                currentLocality.Population <= 1;
        }

        public void Use(Agent agent, Globe globe, IDice dice)
        {
            globe.localitiesCells.TryGetValue(agent.Localtion, out var currentLocality);

            var highestBranchs = currentLocality.Branches.OrderBy(x => x.Value)
                                    .Where(x => /*x.Key != BranchType.Politics &&*/ x.Value >= 1);
            if (highestBranchs.Any())
            {
                var firstBranch = highestBranchs.First();

                TerrainCell freeLocaltion = null;

                for (var freeOffsetX = -1; freeOffsetX <= 1; freeOffsetX++)
                {
                    for (var freeOffsetY = -1; freeOffsetY <= 1; freeOffsetY++)
                    {
                        var freeX = freeOffsetX + currentLocality.Cells[0].X;
                        var freeY = freeOffsetY + currentLocality.Cells[0].Y;

                        if (freeX == 0 && freeY == 0)
                        {
                            continue;
                        }

                        if (freeX < 0)
                        {
                            continue;
                        }

                        if (freeY < 0)
                        {
                            continue;
                        }

                        if (freeX >= globe.Terrain.GetLength(0))
                        {
                            continue;
                        }

                        if (freeY >= globe.Terrain.GetLength(1))
                        {
                            continue;
                        }

                        var freeLocaltion1 = globe.Terrain[freeX][freeY];

                        if (!globe.localitiesCells.TryGetValue(freeLocaltion1, out var freeCheckLocality))
                        {
                            freeLocaltion = globe.Terrain[freeX][freeY];
                        }
                    }
                }

                if (freeLocaltion != null)
                {
                    var createdLocality = new Locality
                    {
                        Name = currentLocality.Name + " " + agent.Name,
                        Branches = new Dictionary<BranchType, int> {
                                                { firstBranch.Key, 1 }
                                            },
                        Cells = new[] {
                                                freeLocaltion
                                            },
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

                    agent.Localtion = rolledTransportLocality.Cells[0];

                    Helper.AddAgentToCell(globe.agentCells, agent.Localtion, agent);
                }
            }
        }
    }
}
