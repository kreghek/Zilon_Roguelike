using System.Linq;
using System.Threading.Tasks;
using Zilon.Core.MapGenerators.PrimitiveStyle;
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
    /// <seealso cref="IDungeonSectorGenerator" />
    public class SectorGenerator : ISectorGenerator
    {

        private readonly IMapFactory _mapFactory;
        private readonly IChestGenerator _chestGenerator;
        private readonly IBotPlayer _monsterPlayer;
        private readonly ISchemeService _schemeService;
        private readonly ITraderManager _traderManager;
        private readonly IDropResolver _dropResolver;
        private readonly ISectorFactory _sectorFactory;
        private readonly IMonsterGenerator _monsterGenerator;

        /// <summary>
        /// Создаёт экземпляр <see cref="SectorGenerator"/>.
        /// </summary>
        /// <param name="mapFactory"> Фабрика карты. Сейчас используется <see cref="RoomStyle.RoomMapFactory"/>. </param>
        /// <param name="sectorFactory"> Фабрика сектора. </param>
        /// <param name="monsterGenerator"> Генератор монстров для подземелий. </param>
        /// <param name="chestGenerator"> Генератор сундуков для подземеоий </param>
        /// <param name="monsterPlayer"> Игрок, управляющий монстрами. </param>
        /// <param name="schemeService"> Сервис схем. </param>
        public SectorGenerator(
            IMapFactory mapFactory,
            ISectorFactory sectorFactory,
            IMonsterGenerator monsterGenerator,
            IChestGenerator chestGenerator,
            IBotPlayer monsterPlayer,
            ISchemeService schemeService,
            ITraderManager traderManager,
            IDropResolver dropResolver
            )
        {
            _mapFactory = mapFactory;
            _sectorFactory = sectorFactory;
            _monsterGenerator = monsterGenerator;
            _chestGenerator = chestGenerator;
            _monsterPlayer = monsterPlayer;
            _schemeService = schemeService;
            _traderManager = traderManager;
            _dropResolver = dropResolver;
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

            var monsterRegions = map.Regions.Where(x => x != map.StartRegion);

            _monsterGenerator.CreateMonsters(sector,
                _monsterPlayer,
                monsterRegions,
                sectorScheme);

            _chestGenerator.CreateChests(map, monsterRegions);

            return sector;
        }

        public async Task<ISector> GenerateTownQuarterAsync(Globe globe, GlobeRegionNode globeNode)
        {
            var map = await SquareMapFactory.CreateAsync(10);

            var sector = _sectorFactory.Create(map);


            var traderDropTable = _schemeService.GetScheme<IDropTableScheme>("trader");
            var trader = new Trader(traderDropTable, map.Nodes.ElementAt(10), _dropResolver);
            _traderManager.Add(trader);
            map.HoldNode(trader.Node, trader);

            return sector;
        }

        public async Task<ISector> GenerateWildAsync(Globe globe, GlobeRegionNode globeNode)
        {
            var map = await SquareMapFactory.CreateAsync(10);
            var sector = _sectorFactory.Create(map);

            return sector;
        }
    }
}
