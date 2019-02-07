using System.Linq;

using Zilon.Core.Players;
using Zilon.Core.Schemes;
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
        private readonly IBotPlayer _monsterPlayer;
        private readonly ISectorFactory _sectorFactory;
        private readonly IMonsterGenerator _monsterGenerator;

        /// <summary>
        /// Создаёт экземпляр <see cref="SectorProceduralGenerator"/>.
        /// </summary>
        /// <param name="mapFactory"> Фабрика карты. Сейчас используется <see cref="RoomStyle.RoomMapFactory"/>. </param>
        /// <param name="sectorFactory"> Фабрика сектора. </param>
        /// <param name="monsterGenerator"> Генератор монстров для подземелий. </param>
        /// <param name="chestGenerator"> Генератор сундуков для подземеоий </param>
        /// <param name="monsterPlayer"> Игрок, управляющий монстрами. </param>
        public SectorProceduralGenerator(
            IMapFactory mapFactory,
            ISectorFactory sectorFactory,
            IMonsterGenerator monsterGenerator,
            IChestGenerator chestGenerator,
            IBotPlayer monsterPlayer
            )
        {
            _mapFactory = mapFactory;
            _sectorFactory = sectorFactory;
            _monsterGenerator = monsterGenerator;
            _chestGenerator = chestGenerator;
            _monsterPlayer = monsterPlayer;
        }

        /// <summary>
        /// Создаёт экземпляр сектора подземелий с указанными параметрами.
        /// </summary>
        /// <param name="sectorScheme"> Схема генерации сектора. </param>
        /// <returns> Возвращает экземпляр сектора. </returns>
        public ISector Generate(ISectorSubScheme sectorScheme)
        {
            var map = _mapFactory.Create();

            var sector = _sectorFactory.Create(map);

            var monsterRegions = map.Regions.Where(x => x != map.StartRegion);

            _monsterGenerator.CreateMonsters(sector,
                _monsterPlayer,
                monsterRegions,
                sectorScheme);

            _chestGenerator.CreateChests(map, monsterRegions);

            return sector;
        }
    }
}
