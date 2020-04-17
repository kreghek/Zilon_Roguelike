using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Common;
using Zilon.Core.Graphs;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators
{
    /// <summary>
    /// Сервис для генерации сундуков в секторе.
    /// </summary>
    public class ChestGenerator : IChestGenerator
    {
        private readonly ISchemeService _schemeService;
        private readonly IDropResolver _dropResolver;
        private readonly IChestGeneratorRandomSource _chestGeneratorRandomSource;

        public ChestGenerator(ISchemeService schemeService,
            IDropResolver dropResolver,
            IChestGeneratorRandomSource chestGeneratorRandomSource)
        {
            _schemeService = schemeService ?? throw new ArgumentNullException(nameof(schemeService));
            _dropResolver = dropResolver ?? throw new ArgumentNullException(nameof(dropResolver));
            _chestGeneratorRandomSource = chestGeneratorRandomSource ?? throw new ArgumentNullException(nameof(chestGeneratorRandomSource));
        }

        /// <inheritdoc/>
        /// <summary>
        /// Создать сундуки в секторе.
        /// </summary>
        public void CreateChests(ISector sector, ISectorSubScheme sectorSubScheme, IEnumerable<MapRegion> regions)
        {
            if (sector is null)
            {
                throw new ArgumentNullException(nameof(sector));
            }

            if (sectorSubScheme is null)
            {
                throw new ArgumentNullException(nameof(sectorSubScheme));
            }

            if (regions is null)
            {
                throw new ArgumentNullException(nameof(regions));
            }

            var trashDropTables = GetTrashDropTables(sectorSubScheme);
            var treasuresDropTable = GetTreasuresDropTable();
            var chestCounter = sectorSubScheme.TotalChestCount;

            //TODO В схемах хранить уже приведённое значение пропорции.
            var countChestRatioNormal = 1f / sectorSubScheme.RegionChestCountRatio;
            foreach (var region in regions)
            {
                var maxChestCountRaw = region.Nodes.Length * countChestRatioNormal;
                var maxChestCount = (int)Math.Max(maxChestCountRaw, 1);

                CreateChestsForRegion(region, maxChestCount, sector, trashDropTables, treasuresDropTable, ref chestCounter);
            }
        }

        private void CreateChestsForRegion(
            MapRegion region,
            int maxChestCount,
            ISector sector,
            IDropTableScheme[] trashDropTables,
            IDropTableScheme[] treasuresDropTable,
            ref int chestCounter)
        {
            if (region.Nodes.Length <= 1)
            {
                // Для регионов, где только один узел,
                // не создаём сундуки, иначе проход может быть загорожен.
                // Актуально для фабрики карт на основе клеточного автомата,
                // потому что он может генерить регионы из одного узла.

                //TODO Попробовать проверять соседей узла.
                // Возможно, одноклеточный регион находится в конце тупика.
                // Тогда в нём можно разместить сундук.
                // Критерий доступности такого региона - у узла только одни сосед.
                return;
            }

            var rolledCount = _chestGeneratorRandomSource.RollChestCount(maxChestCount);

            var map = sector.Map;
            var availableNodes = from node in region.Nodes
                                 where !map.Transitions.Keys.Contains(node)
                                 where map.IsPositionAvailableForContainer(node)
                                 select node;

            var openNodes = new List<IGraphNode>(availableNodes);
            for (var i = 0; i < rolledCount; i++)
            {
                CreateChest(openNodes, sector, trashDropTables, treasuresDropTable);

                chestCounter--;

                if (chestCounter <= 0)
                {
                    // лимит сундуков в секторе исчерпан.
                    break;
                }
            }
        }

        private void CreateChest(List<IGraphNode> openNodes, ISector sector, IDropTableScheme[] trashDropTables, IDropTableScheme[] treasuresDropTable)
        {
            // Выбрать из коллекции доступных узлов
            var rollIndex = _chestGeneratorRandomSource.RollNodeIndex(openNodes.Count);
            var map = sector.Map;
            var containerNode = MapRegionHelper.FindNonBlockedNode(openNodes[rollIndex], map, openNodes);
            if (containerNode == null)
            {
                // в этом случае будет сгенерировано на один сундук меньше.
                // узел, с которого не удаётся найти подходящий узел, удаляем,
                // чтобы больше его не анализировать, т.к. всё равно будет такой же исход.
                openNodes.Remove(openNodes[rollIndex]);
                return;
            }

            // Проверка, что сундук не перегораживает проход.
            var isValid = CheckMap(sector, (HexNode)containerNode);
            if (!isValid)
            {
                // в этом случае будет сгенерировано на один сундук меньше.
                // узел, с которого не удаётся найти подходящий узел, удаляем,
                // чтобы больше его не анализировать, т.к. всё равно будет такой же исход.
                openNodes.Remove(openNodes[rollIndex]);
                return;
            }

            openNodes.Remove(containerNode);

            var staticObject = CreateChestStaticObject(trashDropTables, treasuresDropTable, containerNode);
            sector.StaticObjectManager.Add(staticObject);
        }

        private IStaticObject CreateChestStaticObject(IDropTableScheme[] trashDropTables, IDropTableScheme[] treasuresDropTable, IGraphNode containerNode)
        {
            var containerPurpose = _chestGeneratorRandomSource.RollPurpose();
            var container = CreateContainerModuleByPurpose(trashDropTables,
                treasuresDropTable,
                containerNode,
                containerPurpose);

            var staticObject = new StaticObject(containerNode, default);
            staticObject.AddModule(container);
            return staticObject;
        }

        private IPropContainer CreateContainerModuleByPurpose(IDropTableScheme[] trashDropTables, IDropTableScheme[] treasuresDropTable, IGraphNode containerNode, PropContainerPurpose containerPurpose)
        {
            IPropContainer container;
            switch (containerPurpose)
            {
                case PropContainerPurpose.Trash:
                    container = new DropTablePropChest(containerNode, trashDropTables, _dropResolver)
                    {
                        Purpose = PropContainerPurpose.Trash
                    };
                    break;

                case PropContainerPurpose.Treasures:
                    container = new DropTablePropChest(containerNode, treasuresDropTable, _dropResolver)
                    {
                        Purpose = PropContainerPurpose.Treasures
                    };
                    break;

                default:
                    throw new InvalidOperationException($"Не корректное назначение {containerPurpose}.");
            }

            return container;
        }

        private static bool CheckMap(ISector sector, HexNode containerNode)
        {
            var map = sector.Map;
            var currentStaticObjectsNodes = sector.StaticObjectManager.Items.Select(x => x.Node);

            var allNonObstacleNodes = map.Nodes.OfType<HexNode>().ToArray();
            var allNonContainerNodes = allNonObstacleNodes.Where(x => !currentStaticObjectsNodes.Contains(x));
            var allNodes = allNonContainerNodes.ToArray();

            var matrix = new Matrix<bool>(1000, 1000);
            foreach (var node in allNodes)
            {
                var x = node.OffsetCoords.X;
                var y = node.OffsetCoords.Y;
                matrix.Items[x, y] = true;
            }

            // Закрываем проверяемый узел
            matrix.Items[containerNode.OffsetCoords.X, containerNode.OffsetCoords.Y] = false;

            var startNode = allNodes.First();
            var startPoint = startNode.OffsetCoords;
            var floodPoints = HexBinaryFiller.FloodFill(matrix, startPoint);

            foreach (var point in floodPoints)
            {
                matrix.Items[point.X, point.Y] = false;
            }

            foreach (var node in allNodes)
            {
                var x = node.OffsetCoords.X;
                var y = node.OffsetCoords.Y;
                if (matrix.Items[x, y])
                {
                    return false;
                }
            }

            return true;
        }

        private IDropTableScheme[] GetTreasuresDropTable()
        {
            return new[] { _schemeService.GetScheme<IDropTableScheme>("treasures") };
        }

        private IDropTableScheme[] GetTrashDropTables(ISectorSubScheme sectorSubScheme)
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
