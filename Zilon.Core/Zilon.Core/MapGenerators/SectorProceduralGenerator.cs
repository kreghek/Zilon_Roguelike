using System.Linq;

using Zilon.Core.Tactics;

namespace Zilon.Core.MapGenerators
{
    /// <summary>
    /// Реализация генератора сектора для подземелий.
    /// </summary>
    /// <seealso cref="IDungeonSectorGenerator" />
    public class SectorProceduralGenerator : IDungeonSectorGenerator
    {

        private readonly IMapFactory _mapFactory;
        private readonly IChestGenerator _chestGenerator;
        private readonly ISectorFactory _sectorFactory;
        private readonly IMonsterGenerator _monsterGenerator;

        /// <summary>
        /// Создаёт экземпляр <see cref="SectorProceduralGenerator"/>.
        /// </summary>
        /// <param name="mapFactory"> Фабрика карты. Сейчас используется <see cref="RoomStyle.RoomMapFactory"/> </param>
        /// <param name="sectorFactory"> Фабрика сектора. </param>
        /// <param name="monsterGenerator"> Генератор монстров для подземелий. </param>
        /// <param name="chestGenerator"> Генератор сундуков для подземеоий </param>
        public SectorProceduralGenerator(
            IMapFactory mapFactory,
            ISectorFactory sectorFactory,
            IMonsterGenerator monsterGenerator,
            IChestGenerator chestGenerator
            )
        {
            _mapFactory = mapFactory;
            _sectorFactory = sectorFactory;
            _monsterGenerator = monsterGenerator;
            _chestGenerator = chestGenerator;
        }

        /// <summary>
        /// Создаёт экземпляр сектора подземелий с указанными параметрами.
        /// </summary>
        /// <param name="options"> Параметры генерации.
        /// Сейчас передаётся <see cref="SectorProceduralGeneratorOptions"/> с указанием схем монстров. </param>
        /// <returns> Возвращает экземпляр сектора. </returns>
        public ISector Generate(ISectorGeneratorOptions options)
        {
            var map = _mapFactory.Create();

            var sector = _sectorFactory.Create(map);

            var proceduralOptions = (SectorProceduralGeneratorOptions)options;

            var monsterRegions = map.Regions.Where(x => x != map.StartRegion);

            _monsterGenerator.CreateMonsters(sector,
                monsterRegions,
                proceduralOptions.MonsterGeneratorOptions);

            _chestGenerator.CreateChests(map, monsterRegions);

            return sector;
        }
    }
}
