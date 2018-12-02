using System.Linq;
using System.Text;

using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common;

namespace Zilon.Core.MapGenerators
{
    public class SectorSchemeGenerator : ISectorProceduralGenerator
    {
        private readonly IActorManager _actorManager;
        private readonly IPropContainerManager _propContainerManager;
        private readonly ISectorGeneratorRandomSource _randomSource;
        private readonly IBotPlayer _botPlayer;
        private readonly ISchemeService _schemeService;
        private readonly IPropFactory _propFactory;
        private readonly IDropResolver _dropResolver;

        public StringBuilder Log { get; }

        public SectorSchemeGenerator(IActorManager actorManager,
            IPropContainerManager propContainerManager,
            ISectorGeneratorRandomSource randomSource,
            IBotPlayer botPlayer,
            ISchemeService schemeService,
            IPropFactory propFactory,
            IDropResolver dropResolver)
        {
            _actorManager = actorManager;
            _propContainerManager = propContainerManager;
            _randomSource = randomSource;
            _botPlayer = botPlayer;
            _schemeService = schemeService;
            _propFactory = propFactory;
            _dropResolver = dropResolver;

            Log = new StringBuilder();
        }

        public ISector Generate()
        {
            var map = SquareMapFactory.Create(20);

            var sector = new Sector(map,
                _actorManager,
                _propContainerManager,
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
