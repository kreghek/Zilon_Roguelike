using System;
using System.Linq;
using System.Threading.Tasks;

using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.World;

namespace Zilon.Core.MapGenerators
{
    /// <summary>
    /// Реализация генератора сектора для подземелий.
    /// </summary>
    /// <seealso cref="ISectorGenerator" />
    public class SectorGenerator : ISectorGenerator
    {
        private readonly IStaticObstaclesGenerator _staticObstaclesGenerator;
        private readonly IDiseaseGenerator _diseaseGenerator;
        private readonly IBotPlayer _botPlayer;
        private readonly IResourceMaterializationMap _resourceMaterializationMap;
        private readonly IMapFactorySelector _mapFactorySelector;
        private readonly ISectorFactory _sectorFactory;
        private readonly IMonsterGenerator _monsterGenerator;

        /// <summary>
        /// Создаёт экземпляр <see cref="SectorGenerator"/>.
        /// </summary>
        /// <param name="mapFactorySelector"> Сервис для выбора фабрики для создания карты. </param>
        /// <param name="sectorFactory"> Фабрика сектора. </param>
        /// <param name="monsterGenerator"> Генератор монстров для подземелий. </param>
        /// <param name="chestGenerator"> Генератор сундуков для подземеоий </param>
        /// <param name="botPlayer"> Игрок, управляющий монстрами, мирными жителями. </param>
        public SectorGenerator(
            IMapFactorySelector mapFactorySelector,
            ISectorFactory sectorFactory,
            IMonsterGenerator monsterGenerator,
            IStaticObstaclesGenerator staticObstaclesGenerator,
            IDiseaseGenerator diseaseGenerator,
            IBotPlayer botPlayer,
            IResourceMaterializationMap resourceMaterializationMap)
        {
            _mapFactorySelector = mapFactorySelector ?? throw new ArgumentNullException(nameof(mapFactorySelector));
            _sectorFactory = sectorFactory ?? throw new ArgumentNullException(nameof(sectorFactory));
            _monsterGenerator = monsterGenerator ?? throw new ArgumentNullException(nameof(monsterGenerator));
            _staticObstaclesGenerator = staticObstaclesGenerator ?? throw new ArgumentNullException(nameof(staticObstaclesGenerator));
            _diseaseGenerator = diseaseGenerator ?? throw new ArgumentNullException(nameof(diseaseGenerator));
            _botPlayer = botPlayer ?? throw new ArgumentNullException(nameof(botPlayer));
            _resourceMaterializationMap = resourceMaterializationMap ?? throw new ArgumentNullException(nameof(resourceMaterializationMap));
        }

        /// <summary>
        /// Создаёт экземпляр сектора подземелий с указанными параметрами.
        /// </summary>
        /// <param name="sectorNode"> Схема генерации сектора. </param>
        /// <returns> Возвращает экземпляр сектора. </returns>
        public async Task<ISector> GenerateAsync(ISectorNode sectorNode)
        {
            if (sectorNode is null)
            {
                throw new ArgumentNullException(nameof(sectorNode));
            }

            var mapFactory = _mapFactorySelector.GetMapFactory(sectorNode);

            var transitions = MapFactoryHelper.CreateTransitions(sectorNode);

            var sectorFactoryOptions = new SectorMapFactoryOptions(sectorNode.SectorScheme.MapGeneratorOptions, transitions);

            var map = await mapFactory.CreateAsync(sectorFactoryOptions).ConfigureAwait(false);

            var locationScheme = sectorNode.Biome.LocationScheme;

            var sector = _sectorFactory.Create(map, locationScheme);

            DefineDiseases(sector);

            var gameObjectRegions = map.Regions.Where(x => !x.IsStart).ToArray();

            var sectorScheme = sectorNode.SectorScheme;

            var resourceDepositData = _resourceMaterializationMap.GetDepositData(sectorNode);

            var staticObjectgenerationContext = new StaticObjectGenerationContext(sector, sectorScheme, resourceDepositData);

            await _staticObstaclesGenerator.CreateAsync(staticObjectgenerationContext).ConfigureAwait(false);

            var monsterRegions = gameObjectRegions.ToArray();
            _monsterGenerator.CreateMonsters(sector,
                _botPlayer,
                monsterRegions,
                sectorScheme);

            return sector;
        }

        private void DefineDiseases(ISector sector)
        {
            var disease = _diseaseGenerator.Create();

            if (disease is null)
            {
                return;
            }

            sector.AddDisease(disease);
        }
    }
}
