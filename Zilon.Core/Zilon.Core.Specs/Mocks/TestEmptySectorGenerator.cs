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
        private readonly ISchemeService _schemeService;
        private readonly IMapFactory _mapFactory;
        private readonly IEquipmentDurableService _equipmentDurableService;

        public TestEmptySectorGenerator(
            IDropResolver dropResolver,
            ISchemeService schemeService,
            IMapFactory mapFactory,
            IEquipmentDurableService equipmentDurableService)
        {
            _dropResolver = dropResolver ?? throw new System.ArgumentNullException(nameof(dropResolver));
            _schemeService = schemeService ?? throw new System.ArgumentNullException(nameof(schemeService));
            _mapFactory = mapFactory ?? throw new System.ArgumentNullException(nameof(mapFactory));
            _equipmentDurableService = equipmentDurableService ?? throw new System.ArgumentNullException(nameof(equipmentDurableService));
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
