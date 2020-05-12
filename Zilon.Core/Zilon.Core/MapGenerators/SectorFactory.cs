using System;

using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators
{
    public class SectorFactory : ISectorFactory
    {
        private readonly IDropResolver _dropResolver;
        private readonly ISchemeService _schemeService;
        private readonly IEquipmentDurableService _equipmentDurableService;

        public SectorFactory(IDropResolver dropResolver,
            ISchemeService schemeService,
            IEquipmentDurableService equipmentDurableService)
        {
            _dropResolver = dropResolver ?? throw new ArgumentNullException(nameof(dropResolver));
            _schemeService = schemeService ?? throw new ArgumentNullException(nameof(schemeService));
            _equipmentDurableService = equipmentDurableService;
        }

        public ISector Create(ISectorMap map, ILocationScheme locationScheme)
        {
            var actorManager = new ActorManager();
            var propContainerManager = new StaticObjectManager();

            var sector = new Sector(map,
                actorManager,
                propContainerManager,
                _dropResolver,
                _schemeService,
                _equipmentDurableService)
            {
                Scheme = locationScheme
            };

            return sector;
        }
    }
}
