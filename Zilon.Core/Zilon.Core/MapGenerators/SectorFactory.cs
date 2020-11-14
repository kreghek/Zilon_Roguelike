using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Scoring;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators
{
    public class SectorFactory : ISectorFactory
    {
        private readonly IDropResolver _dropResolver;
        private readonly IEquipmentDurableService _equipmentDurableService;
        private readonly ISchemeService _schemeService;

        public SectorFactory(IDropResolver dropResolver,
            ISchemeService schemeService,
            IEquipmentDurableService equipmentDurableService)
        {
            _dropResolver = dropResolver ?? throw new ArgumentNullException(nameof(dropResolver));
            _schemeService = schemeService ?? throw new ArgumentNullException(nameof(schemeService));
            _equipmentDurableService = equipmentDurableService;
        }

        public IScoreManager ScoreManager { get; set; }

        public NationalUnityEventService NationalUnityEventService { get; set; }

        public ISector Create(ISectorMap map, ILocationScheme locationScheme)
        {
            ActorManager actorManager = new ActorManager();
            StaticObjectManager propContainerManager = new StaticObjectManager();

            Sector sector = new Sector(map,
                actorManager,
                propContainerManager,
                _dropResolver,
                _schemeService,
                _equipmentDurableService)
            {
                Scheme = locationScheme,
                ScoreManager = ScoreManager,
                NationalUnityEventService = NationalUnityEventService
            };

            return sector;
        }
    }
}