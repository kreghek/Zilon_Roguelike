using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Graphs;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.World.GlobeDrafting;

namespace Zilon.Core.World
{
    /// <summary>
    /// Экземпляр генератора мира с историей.
    /// </summary>
    /// <seealso cref="IGlobeGenerator" />
    public sealed class GlobeGenerator : IGlobeGenerator
    {
        private readonly GlobeDraftGenerator _globeDraftGenerator;
        private readonly TerrainInitiator _terrainInitiator;
        private readonly ISectorBuilderFactory _sectorBuilderFactory;
        private readonly IHumanPersonFactory _humanPersonFactory;
        private readonly IBotPlayer _botPlayer;

        private readonly NameGeneration.RandomName _personNameGenerator;

        /// <summary>
        /// Создаёт экземпляр <see cref="GlobeGenerator"/>.
        /// </summary>
        public GlobeGenerator(
            GlobeDraftGenerator globeDraftGenerator,
            TerrainInitiator terrainInitiator,
            ISectorBuilderFactory sectorBuilderFactory,
            IHumanPersonFactory humanPersonFactory,
            IBotPlayer botPlayer)
        {
            _globeDraftGenerator = globeDraftGenerator;
            _terrainInitiator = terrainInitiator;
            _sectorBuilderFactory = sectorBuilderFactory;
            _humanPersonFactory = humanPersonFactory;
            _botPlayer = botPlayer;

            //TODO 
            var dice = new LinearDice();
            _personNameGenerator = new NameGeneration.RandomName(dice);
        }

        public async Task<GlobeGenerationResult> CreateGlobeAsync()
        {
            var globeDraft = _globeDraftGenerator.Generate();

            var globe = new Globe();

            var terrain = await _terrainInitiator.GenerateAsync(globeDraft).ConfigureAwait(false);
            globe.Terrain = terrain;

            var personId = 1;
            foreach (var terrainNode in globe.Terrain.TerrainNodes)
            {
                var province = terrainNode.Province;
                foreach (var provinceNode in province.ProvinceNodes)
                {
                    //TODO Нужно разделить генерацию узлов провинций и генерацию стартовой доп информации.
                    // Под стартовой инфой подразумевается:
                    // - Есть ли на старте мира в узле город? И какой фракции, численности.
                    // - Есть ли на старте мира в узле данж. Если есть, то его схема.
                    // Сейчас эта хранится прямо в узлах провинции. И остаётся там даже после стартовой генерации мира.

                    var sectorBuilder = _sectorBuilderFactory.GetBuilder(provinceNode);
                    var sector = await sectorBuilder.CreateSectorAsync().ConfigureAwait(false);

                    provinceNode.BindSector(sector);

                    //TODO От SectorInfo можно отказаться. Вместо этого достаточно проходится по всем узлам всех провинций.
                    // Если для узла провинции привязан сектор (не null), тогда обрабатывать этот сектор.
                    // Для оптимизации можно хранить отдельный список узлов, к которым привязан сектор. Чтобы
                    // не выполнять обход абсолютчно всех узлов.
                    var sectorInfo = new SectorInfo(sector,
                                                    province,
                                                    provinceNode);

                    globe.SectorInfos.Add(sectorInfo);

                    var terrainNodeCoords = new OffsetCoords(terrainNode.OffsetX, terrainNode.OffsetY);
                    var startLocalityDraft = globeDraft.StartLocalities.SingleOrDefault(x=>x.StartTerrainCoords == terrainNodeCoords);

                    if (startLocalityDraft != null)
                    {
                        for (var populationUnitIndex = 0; populationUnitIndex < startLocalityDraft.Population; populationUnitIndex++)
                        {
                            var node = sector.Map.Nodes.ElementAt(5_050 + populationUnitIndex);
                            var person = CreatePerson(_humanPersonFactory, _personNameGenerator);
                            person.Id = personId;
                            var actor = CreateActor(_botPlayer, person, node);
                            sector.ActorManager.Add(actor);

                            personId++;
                        }
                    }
                }
            };

            var result = new GlobeGenerationResult(globe);

            return result;
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
