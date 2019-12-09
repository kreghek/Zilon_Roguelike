using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Graphs;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Tactics;

namespace Zilon.Core.World
{
    /// <summary>
    /// Экземпляр генератора мира с историей.
    /// </summary>
    /// <seealso cref="IGlobeGenerator" />
    public class GlobeGenerator : IGlobeGenerator
    {
        private readonly TerrainInitiator _terrainInitiator;
        private readonly ProvinceInitiator _provinceInitiator;
        private readonly ISectorBuilderFactory _sectorBuilderFactory;
        private readonly IHumanPersonFactory _humanPersonFactory;
        private readonly IBotPlayer _botPlayer;

        private NameGeneration.RandomName _personNameGenerator;

        /// <summary>
        /// Создаёт экземпляр <see cref="GlobeGenerator"/>.
        /// </summary>
        public GlobeGenerator(
            TerrainInitiator terrainInitiator,
            ProvinceInitiator provinceInitiator,
            ISectorBuilderFactory sectorBuilderFactory,
            IHumanPersonFactory humanPersonFactory,
            IBotPlayer botPlayer)
        {
            _terrainInitiator = terrainInitiator;
            _provinceInitiator = provinceInitiator;
            _sectorBuilderFactory = sectorBuilderFactory;
            _humanPersonFactory = humanPersonFactory;
            _botPlayer = botPlayer;

            //TODO 
            var dice = new LinearDice();
            _personNameGenerator = new NameGeneration.RandomName(dice);
        }

        public async Task<GlobeGenerationResult> CreateGlobeAsync()
        {
            var globe = new Globe();

            var terrain = await _terrainInitiator.GenerateAsync().ConfigureAwait(false);
            globe.Terrain = terrain;

            const int WORLD_SIZE = 40;
            await GenerateAndAssignRegionsAsync(globe, WORLD_SIZE).ConfigureAwait(false);

            const int START_LOCALITIES = 8;
            const int POPULATION_UNIT_COUNT = 4;
            const int PERSON_PER_POPULATION_UNIT = 10;

            // Берём START_LOCALITIES случайных точек. Это стартовые города государсв.
            var localityCoords = Enumerable.Range(0, WORLD_SIZE * WORLD_SIZE)
                .Take(START_LOCALITIES)
                .OrderBy(x => Guid.NewGuid())
                .Select(coordIndex => new Core.OffsetCoords(coordIndex / WORLD_SIZE, coordIndex % WORLD_SIZE));

            var personId = 1;
            foreach (var region in globe.Terrain.Regions)
            {
                var needToCreateSector = localityCoords.Contains(region.TerrainCell.Coords);

                if (needToCreateSector)
                {
                    var sectorBuilder = _sectorBuilderFactory.GetBuilder();
                    var sector = await sectorBuilder.CreateSectorAsync().ConfigureAwait(false);

                    var regionNode = region.RegionNodes.First();
                    regionNode.Sector = sector;

                    var sectorInfo = new SectorInfo(sector,
                                                    region,
                                                    regionNode);
                    globe.SectorInfos.Add(sectorInfo);

                    for (var populationUnitIndex = 0; populationUnitIndex < POPULATION_UNIT_COUNT; populationUnitIndex++)
                    {
                        for (var personIndex = 0; personIndex < PERSON_PER_POPULATION_UNIT; personIndex++)
                        {
                            var node = sector.Map.Nodes.ElementAt(5_050 + personIndex + (populationUnitIndex * PERSON_PER_POPULATION_UNIT));
                            var person = CreatePerson(_humanPersonFactory, _personNameGenerator);
                            person.Id = personId++;
                            var actor = CreateActor(_botPlayer, person, node);
                            sector.ActorManager.Add(actor);
                        }
                    }
                }
            };

            var result = new GlobeGenerationResult(globe);

            return result;
        }

        private async Task GenerateAndAssignRegionsAsync(Globe globe, int WORLD_SIZE)
        {
            var provinces = new ConcurrentBag<GlobeRegion>();
            for (var terrainCellX = 0; terrainCellX < WORLD_SIZE; terrainCellX++)
            {
                for (var terrainCellY = 0; terrainCellY < WORLD_SIZE; terrainCellY++)
                {
                    var province = await CreateProvinceAsync().ConfigureAwait(false);
                    province.TerrainCell = new TerrainCell { Coords = new Core.OffsetCoords(terrainCellX, terrainCellY) };
                    provinces.Add(province);
                }
            };

            globe.Terrain.Regions = provinces.ToArray();
        }

        private async Task<GlobeRegion> CreateProvinceAsync()
        {
            var region = await _provinceInitiator.GenerateRegionAsync().ConfigureAwait(false);
            return region;
        }

        private static IPerson CreatePerson(IHumanPersonFactory humanPersonFactory, NameGeneration.RandomName randomName)
        {
            var person = humanPersonFactory.Create();
            person.Name = randomName.Generate(NameGeneration.Sex.Male);
            return person;
        }

        private static IActor CreateActor(IPlayer personPlayer,
            IPerson person,
            IGraphNode node)
        {
            var actor = new Actor(person, personPlayer, node);

            return actor;
        }
    }
}
