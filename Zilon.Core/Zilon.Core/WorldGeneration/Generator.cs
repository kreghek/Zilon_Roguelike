using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.WorldGeneration
{
    public class Generator
    {
        private readonly IDice _dice;

        public Generator(IDice dice)
        {
            _dice = dice;
        }

        public void Generate()
        {
            var realms = new Realm[4];
            var realmColors = new[] { Color.Red, Color.Green, Color.Blue, Color.Yellow };
            for (var i = 0; i < 4; i++)
            {
                realms[i] = new Realm
                {
                    Name = $"realm {i}",
                    Color = realmColors[i]
                };
            }


            var globe = new Globe
            {
                Terrain = new TerrainCell[100][]
            };

            var localitiesCells = new Dictionary<TerrainCell, Locality>();
            var localities = new List<Locality>();
            for (var i = 0; i < 100; i++)
            {
                globe.Terrain[i] = new TerrainCell[100];

                for (var j = 0; j < 100; j++)
                {
                    globe.Terrain[i][j] = new TerrainCell {
                        X = i,
                        Y = j
                    };
                }
            }

            for (var i = 0; i < 4; i++)
            {
                var randomX = _dice.Roll(0, 100);
                var randomY = _dice.Roll(0, 100);

                var locality = new Locality()
                {
                    Cells = new[] { globe.Terrain[randomX][randomY] },
                    Owner = realms[i],
                    Population = 3
                };

                var rolledBranchIndex = _dice.Roll(0, 7);
                locality.Branches = new Dictionary<BranchType, int>
                        {
                            { (BranchType)rolledBranchIndex, 1 }
                        };

                localities.Add(locality);

                localitiesCells[locality.Cells[0]] = locality;
            }

            var agents = new List<Agent>();
            var agentCells = new Dictionary<TerrainCell, List<Agent>>();
            for (var i = 0; i < 40; i++)
            {
                var rolledLocalityIndex = _dice.Roll(0, localities.Count - 1);
                var locality = localities[rolledLocalityIndex];

                var agent = new Agent
                {
                    Name = $"agent {i}",
                    Localtion = locality.Cells[0],
                    Realm = locality.Owner
                };

                agents.Add(agent);

                AddAgentToCell(agentCells, locality.Cells[0], agent);

                var rolledBranchIndex = _dice.Roll(0, 7);
                agent.Skills = new Dictionary<BranchType, int>
                {
                    { (BranchType)rolledBranchIndex, 1 }
                };
            }

            // обработка итераций
            for (var year = 0; year < 40_000; year++)
            {
                // обработка карты
                var scanResult = new ScanResult();
                for (var x = 0; x < 100; x++)
                {
                    for (var y = 0; y < 100; y++)
                    {

                        var free = true;

                        for (var i = -1; i <= 1; i++)
                        {
                            for (var j = -1; j <= 1; j++)
                            {
                                var scanX = x + i;
                                var scanY = y + i;

                                if (i == 0 && j == 0)
                                {
                                    continue;
                                }

                                if (scanX < 0)
                                {
                                    continue;
                                }

                                if (scanY < 0)
                                {
                                    continue;
                                }

                                if (scanX >= 100)
                                {
                                    continue;
                                }

                                if (scanY >= 100)
                                {
                                    continue;
                                }

                                var scanTerrainCell = globe.Terrain[scanX][scanY];
                                if (localitiesCells.ContainsKey(scanTerrainCell))
                                {
                                    free = false;
                                }
                            }

                            if (!free)
                            {
                                break;
                            }
                        }

                        if (free)
                        {
                            scanResult.Free.Add(globe.Terrain[x][y]);
                        }
                    }
                }


                foreach (var agent in agents.ToArray())
                {
                    if (localitiesCells.TryGetValue(agent.Localtion, out var currentLocality))
                    {
                        // Деятель в городе

                        if (currentLocality.Population <= 1)
                        {
                            currentLocality.Population++;
                            continue;
                        }

                        // если политик на 2 любая компетенция на 1, то
                        // население в городе 3, тогда новый город
                        var politicLevel = 2;
                        //if (currentLocality.Branches.TryGetValue(BranchType.Politics, out var politicLevel))
                        {
                            if (politicLevel >= 2)
                            {
                                var highestBranchs = currentLocality.Branches.OrderBy(x => x.Value)
                                    .Where(x => /*x.Key != BranchType.Politics &&*/ x.Value >= 1);
                                if (highestBranchs.Any())
                                {
                                    var firstBranch = highestBranchs.First();

                                    TerrainCell freeLocaltion = null;

                                    for (var freeX = -1; freeX <= 1; freeX++)
                                    {
                                        for (var freeY = -1; freeY <= 1; freeY++)
                                        {
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

                                            if (freeX >= 100)
                                            {
                                                continue;
                                            }

                                            if (freeY >= 100)
                                            {
                                                continue;
                                            }

                                            var freeLocaltion1 = globe.Terrain[freeX][freeY];

                                            if (!localitiesCells.TryGetValue(freeLocaltion1, out var freeCheckLocality))
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

                                        localities.Add(createdLocality);
                                        localitiesCells[freeLocaltion] = createdLocality;
                                    }
                                    else
                                    {
                                        var realmLocalities = localities.Where(x => x.Owner == agent.Realm).ToArray();
                                        var rolledTransportLocalityIndex = _dice.Roll(0, realmLocalities.Length - 1);
                                        var rolledTransportLocality = realmLocalities[rolledTransportLocalityIndex];

                                        RemoveAgentToCell(agentCells, agent.Localtion, agent);

                                        agent.Localtion = rolledTransportLocality.Cells[0];

                                        AddAgentToCell(agentCells, agent.Localtion, agent);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // за городом
                        // пока ничего не сделано
                    }
                }
            }


            var branchColors = new[] { Color.Red, Color.Blue, Color.Green, Color.Yellow,
                Color.Black, Color.Magenta, Color.Maroon, Color.LightGray };
            using (var realmBmp = new Bitmap(100, 100))
            using (var branchmBmp = new Bitmap(100, 100))
            {
                for (var i = 0; i < 100; i++)
                {
                    for (var j = 0; j < 100; j++)
                    {
                        var cell = globe.Terrain[i][j];
                        if (localitiesCells.TryGetValue(cell, out var locality))
                        {
                            var branch = locality.Branches.Single(x => x.Value > 0);

                            var owner = locality.Owner;
                            realmBmp.SetPixel(i, j, owner.Color);
                            branchmBmp.SetPixel(i, j, branchColors[(int)branch.Key]);
                        }
                        else
                        {
                            realmBmp.SetPixel(i, j, Color.White);
                            branchmBmp.SetPixel(i, j, Color.White);
                        }
                    }
                }

                realmBmp.Save(@"c:\worldgen\realms.bmp", ImageFormat.Bmp);
                branchmBmp.Save(@"c:\worldgen\branches.bmp", ImageFormat.Bmp);
            }
        }

        private class ScanResult
        {
            public HashSet<TerrainCell> Free = new HashSet<TerrainCell>();
        }

        private void AddAgentToCell(Dictionary<TerrainCell, List<Agent>> cells, TerrainCell cell, Agent agent)
        {
            if (cells.TryGetValue(cell, out var list))
            {
                list.Add(agent);
            }
            else
            {
                list = new List<Agent> { agent };
                cells[cell] = list;
            }
        }

        private void RemoveAgentToCell(Dictionary<TerrainCell, List<Agent>> cells, TerrainCell cell, Agent agent)
        {
            if (cells.TryGetValue(cell, out var list))
            {
                list.Remove(agent);
            }
        }
    }
}
