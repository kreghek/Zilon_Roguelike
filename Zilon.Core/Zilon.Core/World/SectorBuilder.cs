using System.Threading.Tasks;

using Zilon.Core.MapGenerators;
using Zilon.Core.MapGenerators.WildStyle;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;

namespace Zilon.Core.World
{
    public class SectorBuilder: ISectorBuilder
    {
        private readonly IMapFactory _mapFactory;
        private readonly IActorManager _actorManager;
        private readonly IPropContainerManager _propContainerManager;
        private readonly IDropResolver _dropResolver;
        private readonly ISchemeService _schemeService;
        private readonly IEquipmentDurableService _equipmentDurableService;

        public SectorBuilder(IMapFactory mapFactory,
            IActorManager actorManager,
            IPropContainerManager propContainerManager,
            IDropResolver dropResolver,
            ISchemeService schemeService,
            IEquipmentDurableService equipmentDurableService)
        {
            _mapFactory = mapFactory;
            _actorManager = actorManager;
            _propContainerManager = propContainerManager;
            _dropResolver = dropResolver;
            _schemeService = schemeService;
            _equipmentDurableService = equipmentDurableService;
        }

        public Task<ISector> CreateSectorAsync() {
            return CreateWildSectorAsync(
                _mapFactory,
                _actorManager,
                _propContainerManager,
                _dropResolver,
                _schemeService,
                _equipmentDurableService);
        }

        private static async Task<ISector> CreateWildSectorAsync(IMapFactory mapFactory,
            IActorManager actorManager,
            IPropContainerManager propContainerManager,
            IDropResolver dropResolver,
            ISchemeService schemeService,
            IEquipmentDurableService equipmentDurableService)
        {
            var sectorMap = await WildMapFactory.CreateAsync(100).ConfigureAwait(false);
            var sector = new Sector(sectorMap,
                                    actorManager,
                                    propContainerManager,
                                    dropResolver,
                                    schemeService,
                                    equipmentDurableService);
            return sector;
        }
    }
}
