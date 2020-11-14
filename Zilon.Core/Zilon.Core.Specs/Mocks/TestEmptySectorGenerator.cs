using System;
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
        private readonly IDropResolver _dropResolver;
        private readonly IEquipmentDurableService _equipmentDurableService;
        private readonly IMapFactory _mapFactory;
        private readonly ISchemeService _schemeService;

        public TestEmptySectorGenerator(
            IDropResolver dropResolver,
            ISchemeService schemeService,
            IMapFactory mapFactory,
            IEquipmentDurableService equipmentDurableService)
        {
            _dropResolver = dropResolver ?? throw new ArgumentNullException(nameof(dropResolver));
            _schemeService = schemeService ?? throw new ArgumentNullException(nameof(schemeService));
            _mapFactory = mapFactory ?? throw new ArgumentNullException(nameof(mapFactory));
            _equipmentDurableService = equipmentDurableService ??
                                       throw new ArgumentNullException(nameof(equipmentDurableService));
        }

        public async Task<ISector> GenerateAsync(ISectorNode sectorNode)
        {
            var sectorFactoryOptions = new SectorMapFactoryOptions(sectorNode.SectorScheme.MapGeneratorOptions);

            var map = await _mapFactory.CreateAsync(sectorFactoryOptions);

            var actorManager = new ActorManager();
            var staticObjectManager = new StaticObjectManager();

            var sector = new Sector(map,
                actorManager,
                staticObjectManager,
                _dropResolver,
                _schemeService,
                _equipmentDurableService);
            return sector;
        }
    }
}