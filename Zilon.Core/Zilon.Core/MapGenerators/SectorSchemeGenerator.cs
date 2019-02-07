using System.Linq;

using Zilon.Core.MapGenerators.PrimitiveStyle;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators
{
    /// <summary>
    /// Задел под генератор сектора по схеме. ВНИМАНИЕ, он ещё не готов к полноценному использованию.
    /// </summary>
    /// <remarks>
    /// Сейчас он больше нужен для тестов. Например, на клиенте.
    /// </remarks>
    public class SectorSchemeGenerator : ISectorProceduralGenerator
    {
        private readonly IActorManager _actorManager;
        private readonly IPropContainerManager _propContainerManager;
        private readonly ITraderManager _traderManager;
        private readonly ISchemeService _schemeService;
        private readonly IPropFactory _propFactory;
        private readonly IDropResolver _dropResolver;

        public SectorSchemeGenerator(IActorManager actorManager,
            IPropContainerManager propContainerManager,
            ITraderManager traderManager,
            ISchemeService schemeService,
            IPropFactory propFactory,
            IDropResolver dropResolver)
        {
            _actorManager = actorManager;
            _propContainerManager = propContainerManager;
            _traderManager = traderManager;
            _schemeService = schemeService;
            _propFactory = propFactory;
            _dropResolver = dropResolver;
        }

        /// <summary> Создаёт сектора с учётом указанных настроек. </summary>
        /// <param name="sectorScheme"> Настройки создания сектора. </param>
        /// <returns> Возвращает созданный сектор. </returns>
        public ISector Generate(ISectorSubScheme sectorScheme)
        {
            var map = SquareMapFactory.Create(20);

            var sector = new Sector(map,
                _actorManager,
                _propContainerManager,
                _traderManager,
                _dropResolver,
                _schemeService);

            CreateChests(map);

            return sector;
        }

        private void CreateChests(IMap map)
        {
            var swordScheme = _schemeService.GetScheme<IPropScheme>("short-sword");

            var equipment = _propFactory.CreateEquipment(swordScheme);

            var absNodeIndex = map.Nodes.Count();
            var containerNode = map.Nodes.ElementAt(absNodeIndex / 2);
            var container = new FixedPropChest(containerNode, new IProp[] { equipment });
            _propContainerManager.Add(container);
        }
    }
}
