using Zilon.Core.MapGenerators;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;

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

        public TestEmptySectorGenerator(IActorManager actorManager,
            IPropContainerManager propContainerManager,
            ITraderManager traderManager,
            IDropResolver dropResolver,
            ISchemeService schemeService,
            IMapFactory mapFactory)
        {
            _actorManager = actorManager;
            _propContainerManager = propContainerManager;
            _traderManager = traderManager;
            _dropResolver = dropResolver;
            _schemeService = schemeService;
            _mapFactory = mapFactory;
        }

        public ISector Generate(ISectorSubScheme sectorScheme)
        {
            var map = _mapFactory.Create();
            var sector = new Sector(map,
                _actorManager,
                _propContainerManager,
                _traderManager,
                _dropResolver,
                _schemeService);
            return sector;
        }
    }
}
