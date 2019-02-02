using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Schemes;
using Zilon.Core.World;
using Zilon.Core.WorldGeneration.AgentCards;

namespace Zilon.Core.WorldGeneration
{
    public class WorldGenerator : IWorldGenerator
    {
        private const int Size = 10;
        private const int StartRealmCount = 4;
        private const int HistoryIterationCount = 40;
        private const int StartAgentCount = 40;

        private readonly IDice _dice;
        private readonly ISchemeService _schemeService;

        public WorldGenerator(IDice dice, ISchemeService schemeService)
        {
            _dice = dice;
            _schemeService = schemeService;
        }

        public Task<Globe> GenerateGlobeAsync()
        {
            var globe = new Globe
            {
                Terrain = new TerrainCell[Size][]
            };

            var realmTask = CreateRealms(globe);
            var terrainTask = CreateTerrain(globe);

            Task.WaitAll(realmTask, terrainTask);

            CreateStartLocalities(globe);
            CreateStartAgents(globe);

            var agentsClock = new Stopwatch();
            agentsClock.Start();

            var cardQueue = CreateAgentCardQueue();

            // обработка итераций
            ProcessIterations(globe, cardQueue);

            agentsClock.Stop();
            Console.WriteLine(agentsClock.ElapsedMilliseconds / 1f + "s");

            return Task.FromResult(globe);
        }

        private void ProcessIterations(Globe globe, Queue<IAgentCard> cardQueue)
        {
            for (var year = 0; year < HistoryIterationCount; year++)
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
        }

        private static Queue<IAgentCard> CreateAgentCardQueue()
        {
            return new Queue<IAgentCard>(new IAgentCard[] {
                new ChangeLocality(),
                new CreateLocality(),
                new IncreasePopulation(),
                new AgentOpposition(),
                new AgentSupport(),
                new Disciple(),
                new TakeLocation()
            });
        }

        private void CreateStartAgents(Globe globe)
        {
            for (var i = 0; i < StartAgentCount; i++)
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
        }

        private void CreateStartLocalities(Globe globe)
        {
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

                globe.scanResult.Free.Remove(locality.Cell);
            }
        }

        private Task CreateTerrain(Globe globe)
        {
            for (var i = 0; i < Size; i++)
            {
                globe.Terrain[i] = new TerrainCell[Size];

                for (var j = 0; j < Size; j++)
                {
                    globe.Terrain[i][j] = new TerrainCell
                    {
                        Coords = new OffsetCoords(i, j)
                    };

                    globe.scanResult.Free.Add(globe.Terrain[i][j]);
                }
            }

            return Task.CompletedTask;
        }

        private Task CreateRealms(Globe globe)
        {
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

            return Task.CompletedTask;
        }

        public Task<GlobeRegion> GenerateRegionAsync(Globe globe, TerrainCell cell)
        {
            var locationSchemeSids = new[] {
                "rat-hole",
                "rat-kingdom",
                "demon-dungeon",
                "demon-lair",
                "crypt",
                "elder-place",
                "genomass-cave"
            };
            var region = new GlobeRegion();

            for (var x = 0; x < 10; x++)
            {
                for (var y = 0; y < 10; y++)
                {
                    var hasDundeonRoll = _dice.Roll(6);
                    if (hasDundeonRoll > 5)
                    {
                        var locationSidIndex = _dice.Roll(0, locationSchemeSids.Length - 1);
                        var locationSid = locationSchemeSids[locationSidIndex];
                        var locationScheme = _schemeService.GetScheme<ILocationScheme>(locationSid);
                        var node = new GlobeRegionNode(x, y, locationScheme);
                        region.AddNode(node);
                    }
                    else
                    {
                        var locationScheme = _schemeService.GetScheme<ILocationScheme>("forest");
                        var node = new GlobeRegionNode(x, y, locationScheme);
                        region.AddNode(node);
                    }
                }
            }

            return Task.FromResult(region);
        }
    }
}
