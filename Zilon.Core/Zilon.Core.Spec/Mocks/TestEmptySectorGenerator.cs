using System.Threading.Tasks;
using Zilon.Core.MapGenerators;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.World;
using Zilon.Core.WorldGeneration;

namespace Zilon.Core.Spec.Mocks
{
    public class TestEmptySectorGenerator : ISectorGenerator
    {
        private readonly IActorManager _actorManager;
        private readonly IPropContainerManager _propContainerManager;
        private readonly ITraderManager _traderManager;
        private readonly IDropResolver _dropResolver;
        private readonly ISchemeService _schemeService;
        private readonly IMapFactory _mapFactory;
        private readonly IEquipmentDurableService _equipmentDurableService;

        public TestEmptySectorGenerator(IActorManager actorManager,
            IPropContainerManager propContainerManager,
            ITraderManager traderManager,
            IDropResolver dropResolver,
            ISchemeService schemeService,
            IMapFactory mapFactory,
            IEquipmentDurableService equipmentDurableService)
        {
            _actorManager = actorManager;
            _propContainerManager = propContainerManager;
            _traderManager = traderManager;
            _dropResolver = dropResolver;
            _schemeService = schemeService;
            _mapFactory = mapFactory;
            _equipmentDurableService = equipmentDurableService;
        }

        public async Task<ISector> GenerateDungeonAsync(ISectorSubScheme sectorScheme)
        {
            var map = await _mapFactory.CreateAsync(sectorScheme);
            var sector = new Sector(map,
                _actorManager,
                _propContainerManager,
                _traderManager,
                _dropResolver,
                _schemeService,
                _equipmentDurableService);
            return sector;
        }

        public Task<ISector> GenerateTownQuarterAsync(Globe globe, GlobeRegionNode globeNode)
        {
            throw new System.NotImplementedException();
        }

        public Task<ISector> GenerateWildAsync(Globe globe, GlobeRegionNode globeNode)
        {
            throw new System.NotImplementedException();
        }
    }
}
