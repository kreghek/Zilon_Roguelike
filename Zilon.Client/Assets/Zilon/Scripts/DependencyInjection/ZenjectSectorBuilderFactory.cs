using Zenject;

using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.World;

namespace Assets.Zilon.Scripts.DependencyInjection
{
    class ZenjectSectorBuilderFactory : ISectorBuilderFactory
    {
        private readonly DiContainer _diContainer;
        private readonly IDropResolver _dropResolver;
        private readonly ISchemeService _schemeService;
        private readonly IEquipmentDurableService _equipmentDurableService;

        public ZenjectSectorBuilderFactory(
            DiContainer diContainer,
            IDropResolver dropResolver,
            ISchemeService schemeService,
            IEquipmentDurableService equipmentDurableService)
        {
            _diContainer = diContainer;
            _dropResolver = dropResolver;
            _schemeService = schemeService;
            _equipmentDurableService = equipmentDurableService;
        }

        public ISectorBuilder GetBuilder(ProvinceNode provinceNode)
        {
            var actorManager = _diContainer.Resolve<IActorManager>();
            var propContainerManager = _diContainer.Resolve<IPropContainerManager>();

            return new SectorBuilder(
                actorManager,
                propContainerManager,
                _dropResolver,
                _schemeService,
                _equipmentDurableService);
        }
    }
}
