using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators
{
    public class ChestGenerator : IChestGenerator
    {
        private readonly ISchemeService _schemeService;
        private readonly IDropResolver _dropResolver;
        private readonly IPropContainerManager _propContainerManager;
        private readonly IChestGeneratorRandomSource _chestGeneratorRandomSource;

        public ChestGenerator(ISchemeService schemeService, 
            IDropResolver dropResolver,
            IPropContainerManager propContainerManager,
            IChestGeneratorRandomSource chestGeneratorRandomSource)
        {
            _schemeService = schemeService;
            _dropResolver = dropResolver;
            _propContainerManager = propContainerManager;
            _chestGeneratorRandomSource = chestGeneratorRandomSource;
        }

        public void CreateChests(IMap map, IEnumerable<MapRegion> regions)
        {
            var defaultDropTable = _schemeService.GetScheme<IDropTableScheme>("default");
            var survivalDropTable = _schemeService.GetScheme<IDropTableScheme>("survival");

            foreach (var region in regions)
            {
                var maxChestCount = Math.Max(region.Nodes.Count() / 9, 1);
                var rolledCount = _chestGeneratorRandomSource.RollChestCount(maxChestCount);

                var freeNodes = new List<IMapNode>(region.Nodes);
                for (var i = 0; i < rolledCount; i++)
                {
                    // Выбрать из коллекции доступных узлов
                    var rollIndex = _chestGeneratorRandomSource.RollNodeIndex(freeNodes.Count);
                    var containerNode = MapRegionHelper.FindNonBlockedNode(freeNodes[rollIndex], map, freeNodes);
                    if (containerNode == null)
                    {
                        // в этом случае будет сгенерировано на один сундук меньше.
                        // узел, с которого не удаётся найти подходящий узел, удаляем,
                        // чтобы больше его не анализировать, т.к. всё равно будет такой же исход.
                        freeNodes.Remove(freeNodes[rollIndex]);
                        continue;
                    }

                    freeNodes.Remove(containerNode);
                    var container = new DropTablePropChest(containerNode,
                        new[] { defaultDropTable, survivalDropTable },
                        _dropResolver);
                    _propContainerManager.Add(container);
                }
            }
        }
    }
}
