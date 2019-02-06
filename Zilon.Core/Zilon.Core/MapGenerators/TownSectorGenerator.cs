using System.Linq;
using Zilon.Core.MapGenerators.RoomStyle;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;

namespace Zilon.Core.MapGenerators
{
    /// <summary>
    /// Генератор сектора для городской локации.
    /// </summary>
    public sealed class TownSectorGenerator : ITownSectorGenerator
    {
        private readonly IMapFactory _mapFactory;
        private readonly ISectorFactory _sectorFactory;
        private readonly ITraderManager _traderManager;
        private readonly ISchemeService _schemeService;
        private readonly IDropResolver _dropResolver;

        /// <summary>
        /// Конструктор генератора сектора.
        /// </summary>
        /// <param name="mapFactory"> Фабрика карты. Сюда будет передаваться <see cref="TownMapFactory"/> </param>
        /// <param name="sectorFactory"> Фабрика сектора. </param>
        /// <param name="traderManager"> Менеджер торговцев в секторе. </param>
        public TownSectorGenerator(
            IMapFactory mapFactory,
            ISectorFactory sectorFactory,
            ITraderManager traderManager,
            ISchemeService schemeService,
            IDropResolver dropResolver)
        {
            _mapFactory = mapFactory;
            _sectorFactory = sectorFactory;
            _traderManager = traderManager;
            _schemeService = schemeService;
            _dropResolver = dropResolver;
        }

        /// <summary>
        /// Создание сектора.
        /// </summary>
        /// <param name="options"> Настройки генерации сектора. </param>
        /// <returns> Возвращает экземпляр сектора. </returns>
        public ISector Generate(ISectorGeneratorOptions options)
        {
            var map = _mapFactory.Create();

            var sector = _sectorFactory.Create(map);


            var traderDropTable = _schemeService.GetScheme<IDropTableScheme>("trader");
            var trader = new Trader(traderDropTable, map.Nodes.ElementAt(10), _dropResolver);
            _traderManager.Add(trader);
            map.HoldNode(trader.Node, trader);

            return sector;
        }
    }
}
