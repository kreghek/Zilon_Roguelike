using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.World;
using Zilon.Core.WorldGeneration.AgentCards;

namespace Zilon.Core.WorldGeneration
{
    public class WorldGenerator : IWorldGenerator
    {
        private readonly IDice _dice;

        public WorldGenerator(IDice dice)
        {
            _dice = dice;
        }

        public Globe GenerateGlobe()
        {
            const int Size = 50;
            const int StartRealmCount = 4;

            var globe = new Globe
            {
                Terrain = new TerrainCell[Size][],
            };

            var realmColors = new[] { Color.Red, Color.Green, Color.Blue, Color.Yellow };
            for (var i = 0; i < StartRealmCount; i++)
            {
                var realm = new Realm
                {
                    Name = $"realm {i}",
                    Color = realmColors[i]
                };

                globe.Realms.Add(realm);
            }

            var scanResult = new ScanResult();
            for (var i = 0; i < Size; i++)
            {
                globe.Terrain[i] = new TerrainCell[Size];

                for (var j = 0; j < Size; j++)
                {
                    globe.Terrain[i][j] = new TerrainCell
                    {
                        Coords = new OffsetCoords(i, j)
                    };

                    scanResult.Free.Add(globe.Terrain[i][j]);
                }
            }


            for (var i = 0; i < StartRealmCount; i++)
            {
                var randomX = _dice.Roll(0, Size - 1);
                var randomY = _dice.Roll(0, Size - 1);

                var locality = new Locality()
                {
                    Name = $"L{i}",
                    Cell = globe.Terrain[randomX][randomY],
                    Owner = globe.Realms[i],
                    Population = 3
                };

                var rolledBranchIndex = _dice.Roll(0, 7);
                locality.Branches = new Dictionary<BranchType, int>
                        {
                            { (BranchType)rolledBranchIndex, 1 }
                        };

                globe.Localities.Add(locality);

                globe.LocalitiesCells[locality.Cell] = locality;

                scanResult.Free.Remove(locality.Cell);
            }

            for (var i = 0; i < 40; i++)
            {
                var rolledLocalityIndex = _dice.Roll(0, globe.Localities.Count - 1);
                var locality = globe.Localities[rolledLocalityIndex];

                var agent = new Agent
                {
                    Name = $"agent {i}",
                    Localtion = locality.Cell,
                    Realm = locality.Owner
                };

                globe.Agents.Add(agent);

                Helper.AddAgentToCell(globe.AgentCells, locality.Cell, agent);

                var rolledBranchIndex = _dice.Roll(0, 7);
                agent.Skills = new Dictionary<BranchType, int>
                {
                    { (BranchType)rolledBranchIndex, 1 }
                };
            }

            var agentsClock = new Stopwatch();
            agentsClock.Start();

            // обработка итераций
            var cardQueue = new Queue<IAgentCard>(new IAgentCard[] {
                new ChangeLocality(),
                new CreateLocality(),
                new IncreasePopulation(),
                new AgentOpposition(),
                new AgentSupport(),
                new Disciple(),
                new TakeLocation()
            });

            for (var year = 0; year < 40_000; year++)
            {
                foreach (var agent in globe.Agents.ToArray())
                {
                    var card = cardQueue.Dequeue();

                    if (card.CanUse(agent, globe))
                    {
                        card.Use(agent, globe, _dice);
                    }

                    cardQueue.Enqueue(card);
                }
            }

            agentsClock.Stop();
            Console.WriteLine(agentsClock.ElapsedMilliseconds / 1f + "s");

            return globe;
        }

        public GlobeRegion GenerateRegion(Globe globe, TerrainCell cell)
        {
            var sectorSchemeSids = new[] { "rat-post", "rat-kingdom", "demon-dungeon", "demon-lair" };
            var region = new GlobeRegion();

            for (var x = 0; x < 10; x++)
            {
                for (var y = 0; y < 10; y++)
                {
                    var hasDundeonRoll = _dice.Roll(6);
                    if (hasDundeonRoll > 5)
                    {
                        var sectorSidIndex = _dice.Roll(0, sectorSchemeSids.Length - 1);
                        var sectorSid = sectorSchemeSids[sectorSidIndex];
                        var node = new GlobeRegionNode(x, y, sectorSid);
                        region.AddNode(node);
                    }
                    else
                    {
                        var node = new GlobeRegionNode(x, y);
                        region.AddNode(node);
                    }
                }
            }

            return region;
        }
    }
}
