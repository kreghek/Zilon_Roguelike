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

        /// <summary>
        /// Создать сундуки в секторе.
        /// </summary>
        /// <param name="map">Карта сектора. Нужна для определения доступного места для сундука.</param>
        /// <param name="sectorSubScheme">Схема сектора. По сути - настройки для размещения сундуков.</param>
        /// <param name="regions">Регионы, в которых возможно размещение сундуков.</param>
        public void CreateChests(ISectorMap map, ISectorSubScheme sectorSubScheme, IEnumerable<MapRegion> regions)
        {
            var dropTables = GetDropTables(sectorSubScheme);
            var chestCounter = sectorSubScheme.TotalChestCount;

            //TODO В схемах хранить уже приведённое значение пропорции.
            var countChestRatioNormal = 1f / sectorSubScheme.RegionChestCountRatio;
            foreach (var region in regions)
            {
                var maxChestCountRaw = region.Nodes.Count() * countChestRatioNormal;
                var maxChestCount = (int)Math.Max(maxChestCountRaw, 1);
                var rolledCount = _chestGeneratorRandomSource.RollChestCount(maxChestCount);

                var availableNodes = from node in region.Nodes
                                     where !map.Transitions.Keys.Contains(node)
                                     select node;
                var openNodes = new List<IMapNode>(availableNodes);
                for (var i = 0; i < rolledCount; i++)
                {
                    // Выбрать из коллекции доступных узлов
                    var rollIndex = _chestGeneratorRandomSource.RollNodeIndex(openNodes.Count);
                    var containerNode = MapRegionHelper.FindNonBlockedNode(openNodes[rollIndex], map, openNodes);
                    if (containerNode == null)
                    {
                        // в этом случае будет сгенерировано на один сундук меньше.
                        // узел, с которого не удаётся найти подходящий узел, удаляем,
                        // чтобы больше его не анализировать, т.к. всё равно будет такой же исход.
                        openNodes.Remove(openNodes[rollIndex]);
                        continue;
                    }

                    openNodes.Remove(containerNode);
                    var container = new DropTablePropChest(containerNode,
                        dropTables,
                        _dropResolver);
                    _propContainerManager.Add(container);

                    chestCounter--;

                    if (chestCounter <= 0)
                    {
                        // лимит сундуков в секторе исчерпан.
                        break;
                    }
                }
            }
        }

        private IDropTableScheme[] GetDropTables(ISectorSubScheme sectorSubScheme)
        {
            var dropTables = new List<IDropTableScheme>();
            foreach (var chestDropSid in sectorSubScheme.ChestDropTableSids)
            {
                var dropTable = _schemeService.GetScheme<IDropTableScheme>(chestDropSid);
                dropTables.Add(dropTable);
            }

            return dropTables.ToArray();
        }
    }
}
