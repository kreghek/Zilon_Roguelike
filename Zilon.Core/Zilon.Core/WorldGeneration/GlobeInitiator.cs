using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.World;
using Zilon.Core.WorldGeneration.NameGeneration;

namespace Zilon.Core.WorldGeneration
{
    public sealed class GlobeInitiator
    {
        private const int WORLD_SIZE = 40;
        private const int START_ITERATION_REALMS = 8;
        private const int StartAgentCount = 40;

        private readonly string[] _realmNames = new[] {
            "Sons Of The Law",
            "Gaarn Folk",
            "Sun'Ost Union",
            "Hellgrimar Republik",
            "Anklav Of The Seven Seas",
            "Eagle Home Keepers",
            "Cult of Liquid DOG",
            "Free Сities Сouncil"
        };
        private readonly IDice _dice;

        public GlobeInitiator(IDice dice)
        {
            _dice = dice ?? throw new ArgumentNullException(nameof(dice));
        }

        public Task<Globe> CreateStartGlobeAsync()
        {
            return Task.Run(() =>
            {
                var globe = new Globe
                {
                    Terrain = new TerrainCell[WORLD_SIZE][],
                    agentNameGenerator = new RandomName(_dice),
                    cityNameGenerator = new CityNameGenerator(_dice)
                };

                var realmTask = CreateRealmsAsync(globe, _realmNames);
                var terrainTask = CreateTerrainAsync(globe);

                Task.WaitAll(realmTask, terrainTask);

                CreateStartLocalities(globe);
                CreateStartAgents(globe);

                return globe;
            });
        }

        private static Task CreateRealmsAsync(Globe globe, string[] realmNames)
        {
            return Task.Run(() =>
            {
                var realmColors = new[]
                {
                    Color.Red,
                    Color.Green,
                    Color.Blue,
                    Color.Yellow,
                    Color.Beige,
                    Color.LightGray,
                    Color.Magenta,
                    Color.Cyan
                };

                for (var i = 0; i < START_ITERATION_REALMS; i++)
                {
                    var realm = new Realm
                    {
                        Name = realmNames[i],
                        Banner = new RealmBanner { MainColor = realmColors[i] }
                    };

                    globe.Realms.Add(realm);
                }
            });
        }

        private static Task CreateTerrainAsync(Globe globe)
        {
            return Task.Run(() =>
            {
                for (var i = 0; i < WORLD_SIZE; i++)
                {
                    globe.Terrain[i] = new TerrainCell[WORLD_SIZE];

                    for (var j = 0; j < WORLD_SIZE; j++)
                    {
                        globe.Terrain[i][j] = new TerrainCell
                        {
                            Coords = new OffsetCoords(i, j)
                        };

                        var terrain = globe.Terrain[i][j];
                        globe.ScanResult.Free.Add(terrain);
                    }
                }
            });
        }

        private void CreateStartLocalities(Globe globe)
        {
            for (var i = 0; i < START_ITERATION_REALMS; i++)
            {
                var randomX = _dice.Roll(0, WORLD_SIZE - 1);
                var randomY = _dice.Roll(0, WORLD_SIZE - 1);

                var localityName = globe.GetLocalityName(_dice);

                var locality = new Locality()
                {
                    Name = localityName,
                    Cell = globe.Terrain[randomX][randomY],
                    Owner = globe.Realms[i],
                    //Population = 3
                };

                var rolledBranchIndex = _dice.Roll(0, 7);
                locality.Branches = new Dictionary<BranchType, int>
                        {
                            { (BranchType)rolledBranchIndex, 1 }
                        };

                globe.Localities.Add(locality);

                globe.LocalitiesCells[locality.Cell] = locality;

                globe.ScanResult.Free.Remove(locality.Cell);
            }
        }

        private void CreateStartAgents(Globe globe)
        {
            for (var i = 0; i < StartAgentCount; i++)
            {
                var rolledLocalityIndex = _dice.Roll(0, globe.Localities.Count - 1);
                var locality = globe.Localities[rolledLocalityIndex];

                var agentName = globe.agentNameGenerator.Generate(Sex.Male, 1);

                var agent = new Agent
                {
                    Name = agentName,
                    Location = locality.Cell,
                    Realm = locality.Owner
                };

                globe.Agents.Add(agent);

                CacheHelper.AddAgentToCell(globe.AgentCells, locality.Cell, agent);

                var rolledBranchIndex = _dice.Roll(0, 7);
                agent.Skills = new Dictionary<BranchType, int>
                {
                    { (BranchType)rolledBranchIndex, 1 }
                };
            }
        }
    }
}
