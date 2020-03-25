using System.Threading.Tasks;

using Zilon.Core.MapGenerators;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.World;

namespace Zilon.Core.Specs.Mocks
{
    public class TestEmptySectorGenerator : ISectorGenerator
    {
        private readonly IActorManager _actorManager;
        private readonly IPropContainerManager _propContainerManager;
        private readonly IDropResolver _dropResolver;
        private readonly ISchemeService _schemeService;
        private readonly IMapFactory _mapFactory;
        private readonly IEquipmentDurableService _equipmentDurableService;

        public TestEmptySectorGenerator(IActorManager actorManager,
            IPropContainerManager propContainerManager,
            IDropResolver dropResolver,
            ISchemeService schemeService,
            IMapFactory mapFactory,
            IEquipmentDurableService equipmentDurableService)
        {
            _actorManager = actorManager;
            _propContainerManager = propContainerManager;
            _dropResolver = dropResolver;
            _schemeService = schemeService;
            _mapFactory = mapFactory;
            _equipmentDurableService = equipmentDurableService;
        }

        public async Task<ISector> GenerateAsync(ISectorNode sectorNode)
        {
            var map = await _mapFactory.CreateAsync(sectorNode);
            var sector = new Sector(map,
                _actorManager,
                _propContainerManager,
                _dropResolver,
                _schemeService,
                _equipmentDurableService);
            return sector;
        }
    }
}
