using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators
{
    public class ChestGenerator
    {
        private readonly ISchemeService _schemeService;
        private readonly IDropResolver _dropResolver;
        private readonly IPropContainerManager _propContainerManager;

        public ChestGenerator(ISchemeService schemeService, 
            IDropResolver dropResolver,
            IPropContainerManager propContainerManager)
        {
            _schemeService = schemeService;
            _dropResolver = dropResolver;
            _propContainerManager = propContainerManager;
        }

        public void CreateChests(IMap map, IEnumerable<MapRegion> regions)
        {
            var defaultDropTable = _schemeService.GetScheme<IDropTableScheme>("default");
            var survivalDropTable = _schemeService.GetScheme<IDropTableScheme>("survival");

            foreach (var room in regions)
            {
                var containerNode = MapRegionHelper.FindNonBlockedNode(map, room.Nodes);
                var container = new DropTablePropChest(containerNode,
                    new[] { defaultDropTable, survivalDropTable },
                    _dropResolver);
                _propContainerManager.Add(container);
            }
        }
    }
}
