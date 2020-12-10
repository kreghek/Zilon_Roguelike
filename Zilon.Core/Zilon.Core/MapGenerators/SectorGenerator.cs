using System;
using System.Linq;
using System.Threading.Tasks;

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
        private readonly IDiseaseGenerator _diseaseGenerator;
        private readonly IMapFactorySelector _mapFactorySelector;
        private readonly IMonsterGenerator _monsterGenerator;
        private readonly IResourceMaterializationMap _resourceMaterializationMap;
        private readonly ISectorFactory _sectorFactory;
        private readonly IStaticObstaclesGenerator _staticObstaclesGenerator;

        /// <summary>
        /// Создаёт экземпляр <see cref="SectorGenerator" />.
        /// </summary>
        /// <param name="mapFactorySelector"> Сервис для выбора фабрики для создания карты. </param>
        /// <param name="sectorFactory"> Фабрика сектора. </param>
        /// <param name="monsterGenerator"> Генератор монстров для подземелий. </param>
        public SectorGenerator(
            IMapFactorySelector mapFactorySelector,
            ISectorFactory sectorFactory,
            IMonsterGenerator monsterGenerator,
            IStaticObstaclesGenerator staticObstaclesGenerator,
            IDiseaseGenerator diseaseGenerator,
            IResourceMaterializationMap resourceMaterializationMap)
        {
            _mapFactorySelector = mapFactorySelector ?? throw new ArgumentNullException(nameof(mapFactorySelector));
            _sectorFactory = sectorFactory ?? throw new ArgumentNullException(nameof(sectorFactory));
            _monsterGenerator = monsterGenerator ?? throw new ArgumentNullException(nameof(monsterGenerator));
            _staticObstaclesGenerator = staticObstaclesGenerator ??
                                        throw new ArgumentNullException(nameof(staticObstaclesGenerator));
            _diseaseGenerator = diseaseGenerator ?? throw new ArgumentNullException(nameof(diseaseGenerator));
            _resourceMaterializationMap = resourceMaterializationMap ??
                                          throw new ArgumentNullException(nameof(resourceMaterializationMap));
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

            var mapGeneratorOptions = sectorNode.SectorScheme.MapGeneratorOptions;
            var sectorFactoryOptions = new SectorMapFactoryOptions(mapGeneratorOptions, transitions);

            var map = await mapFactory.CreateAsync(sectorFactoryOptions).ConfigureAwait(false);

            var locationScheme = sectorNode.Biome.LocationScheme;

            var sector = _sectorFactory.Create(map, locationScheme);

            DefineDiseases(sector);

            var gameObjectRegions = map.Regions.Where(x => !x.IsStart).ToArray();

            var sectorScheme = sectorNode.SectorScheme;

            await GenerateStaticObjectsAsync(sector, sectorScheme, sectorNode).ConfigureAwait(false);

            var monsterRegions = gameObjectRegions.ToArray();
            _monsterGenerator.CreateMonsters(sector,
                monsterRegions,
                sectorScheme);

            return sector;
        }

        private async Task GenerateStaticObjectsAsync(
            ISector sector,
            Schemes.ISectorSubScheme sectorScheme,
            ISectorNode sectorNode)
        {
            var resourceDepositData = _resourceMaterializationMap.GetDepositData(sectorNode);
            var context = new StaticObjectGenerationContext(sector, sectorScheme, resourceDepositData);

            await _staticObstaclesGenerator.CreateAsync(context).ConfigureAwait(false);
        }
    }
}