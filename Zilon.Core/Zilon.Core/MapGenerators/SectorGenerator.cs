using System.Linq;
using System.Threading.Tasks;

using Zilon.Core.MapGenerators.RoomStyle;
using Zilon.Core.MapGenerators.WildStyle;
using Zilon.Core.Players;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.World;
using Zilon.Core.WorldGeneration;

namespace Zilon.Core.MapGenerators
{
    /// <summary>
    /// Реализация генератора сектора для подземелий.
    /// </summary>
    /// <seealso cref="ISectorGenerator" />
    public class SectorGenerator : ISectorGenerator
    {

        private readonly IMapFactory _mapFactory;
        private readonly IChestGenerator _chestGenerator;
        private readonly IBotPlayer _botPlayer;
        private readonly ICitizenGenerator _citizenGenerator;
        private readonly ISectorFactory _sectorFactory;
        private readonly IMonsterGenerator _monsterGenerator;

        /// <summary>
        /// Создаёт экземпляр <see cref="SectorGenerator"/>.
        /// </summary>
        /// <param name="mapFactory"> Фабрика карты. Сейчас используется <see cref="RoomMapFactory"/>. </param>
        /// <param name="sectorFactory"> Фабрика сектора. </param>
        /// <param name="monsterGenerator"> Генератор монстров для подземелий. </param>
        /// <param name="chestGenerator"> Генератор сундуков для подземеоий </param>
        /// <param name="citizenGenerator"> Генератор жителей в городском квартале. </param>
        /// <param name="botPlayer"> Игрок, управляющий монстрами, мирными жителями. </param>
        public SectorGenerator(
            IMapFactory mapFactory,
            ISectorFactory sectorFactory,
            IMonsterGenerator monsterGenerator,
            IChestGenerator chestGenerator,
            ICitizenGenerator citizenGenerator
,
            IBotPlayer botPlayer)
        {
            _mapFactory = mapFactory;
            _sectorFactory = sectorFactory;
            _monsterGenerator = monsterGenerator;
            _chestGenerator = chestGenerator;
            _botPlayer = botPlayer;
            _citizenGenerator = citizenGenerator;
        }

        /// <summary>
        /// Создаёт экземпляр сектора подземелий с указанными параметрами.
        /// </summary>
        /// <param name="sectorScheme"> Схема генерации сектора. </param>
        /// <returns> Возвращает экземпляр сектора. </returns>
        public async Task<ISector> GenerateDungeonAsync(ISectorSubScheme sectorScheme)
        {
            var map = await _mapFactory.CreateAsync(sectorScheme);

            var sector = _sectorFactory.Create(map);

            var monsterRegions = map.Regions.Where(x => !x.IsStart).ToArray();

            _chestGenerator.CreateChests(map, sectorScheme, monsterRegions);

            _monsterGenerator.CreateMonsters(sector,
                _botPlayer,
                monsterRegions,
                sectorScheme);

            return sector;
        }

        /// <summary>
        /// Создаёт сектор квартала города.
        /// </summary>
        /// <param name="globe">Объект мира.</param>
        /// <param name="globeNode">Узел провинции, на основе которого генерируется сектор.</param>
        /// <returns>
        /// Возвращает созданный сектор.
        /// </returns>
        /// <remarks>
        /// Нужно будет передавать параметры зданий, наличие персонажей и станков для крафта.
        /// Вместо общей информации об узле.
        /// </remarks>
        public async Task<ISector> GenerateTownQuarterAsync(Globe globe, GlobeRegionNode globeNode)
        {
            var townScheme = new TownSectorScheme
            {
                RegionCount = 10,
                RegionSize = 10
            };

            var map = await _mapFactory.CreateAsync(townScheme);

            var sector = _sectorFactory.Create(map);

            _citizenGenerator.CreateCitizens(sector, _botPlayer, sector.Map.Regions);

            //TODO Выходы нужно генерировать в карте, аналогично подземельям.
            map.Transitions.Add(map.Nodes.Last(), RoomTransition.CreateGlobalExit());

            return sector;
        }

        /// <summary>
        /// Создаёт сектор фрагмента дикого окружения.
        /// </summary>
        /// <param name="globe">Объект мира.</param>
        /// <param name="globeNode">Узел провинции, на основе которого генерируется сектор.</param>
        /// <returns>
        /// Возвращает созданный сектор.
        /// </returns>
        /// <remarks>
        /// Нужно будет передавать параметры окружения и количество
        /// и характеристики монстров.
        /// </remarks>
        public async Task<ISector> GenerateWildAsync(Globe globe, GlobeRegionNode globeNode)
        {
            var map = await WildMapFactory.CreateAsync(30);
            var sector = _sectorFactory.Create(map);

            if (globeNode.MonsterState != null)
            {
                _monsterGenerator.CreateMonsters(sector,
                    _botPlayer,
                    new[] { map.Regions[1] },
                    globeNode.MonsterState.MonsterPersons);
            }

            return sector;
        }
    }
}
