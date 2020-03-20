using System.Linq;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Commands.Globe;
using Zilon.Core.Schemes;
using Zilon.Core.World;
using Zilon.Core.WorldGeneration;

namespace Zilon.Core.Tests.Commands.Globe
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class RegionTransitionHelperTests
    {
        /// <summary>
        /// Тест проверяет корректность обнаружения узлов, доступных для перехода в соседнюю провинцию.
        /// Движение по верхней границе направо.
        /// </summary>
        [Test]
        public void GetNeighborBorderNodes_TopRight_ReturnsTopLeft()
        {
            // ARRANGE
            const int REGION_SIZE = 20;
            // У нас есть 2 региона, расположенных по соседству.

            var regionSchemeMock = new Mock<ILocationScheme>();
            var regionScheme = regionSchemeMock.Object;

            var currentRegion = new Province(REGION_SIZE);
            FillRegion(REGION_SIZE, regionScheme, currentRegion);

            // Берём правую верхнюю точку для перехода.
            var currentNode = currentRegion.Nodes.OfType<ProvinceNode>().Single(node => node.OffsetX == REGION_SIZE - 1 && node.OffsetY == 0);

            var currentTerrainCell = new TerrainCell()
            {
                Coords = new OffsetCoords(0, 0)
            };

            var targetRegion = new Province(REGION_SIZE);
            FillRegion(REGION_SIZE, regionScheme, targetRegion);
            var expectedTransitionNodes = new[] {
                targetRegion.Nodes.OfType<ProvinceNode>().Single(node => node.OffsetX == 0 && node.OffsetY == 0)
            };

            var targetRegionBorders = targetRegion.Nodes.OfType<ProvinceNode>().Where(node => node.IsBorder);
            var targetTerrainCell = new TerrainCell()
            {
                Coords = new OffsetCoords(1, 0)
            };

            // ACT
            var factTransitionNodes = RegionTransitionHelper.GetNeighborBorderNodes(currentNode,
                currentTerrainCell,
                targetRegionBorders,
                targetTerrainCell);

            // ASSERT
            factTransitionNodes.Should().BeEquivalentTo(expectedTransitionNodes);
        }

        /// <summary>
        /// Тест проверяет корректность обнаружения узлов, доступных для перехода в соседнюю провинцию.
        /// Движение по верхней границе налево.
        /// </summary>
        [Test]
        public void GetNeighborBorderNodes_TopLeft_ReturnsTopRight()
        {
            // ARRANGE
            const int REGION_SIZE = 20;
            // У нас есть 2 региона, расположенных по соседству.

            var regionSchemeMock = new Mock<ILocationScheme>();
            var regionScheme = regionSchemeMock.Object;

            var currentRegion = new Province(REGION_SIZE);
            FillRegion(REGION_SIZE, regionScheme, currentRegion);

            // Берём правую верхнюю точку для перехода.
            var currentNode = currentRegion.Nodes.OfType<ProvinceNode>().Single(node => node.OffsetX == 0 && node.OffsetY == 0);

            var currentTerrainCell = new TerrainCell()
            {
                Coords = new OffsetCoords(1, 0)
            };

            var targetRegion = new Province(REGION_SIZE);
            FillRegion(REGION_SIZE, regionScheme, targetRegion);
            var expectedTransitionNodes = new[] {
                targetRegion.Nodes.OfType<ProvinceNode>().Single(node => node.OffsetX == REGION_SIZE - 1 && node.OffsetY == 0),
                targetRegion.Nodes.OfType<ProvinceNode>().Single(node => node.OffsetX == REGION_SIZE - 1 && node.OffsetY == 1)
            };

            var targetRegionBorders = targetRegion.Nodes.OfType<ProvinceNode>().Where(node => node.IsBorder);
            var targetTerrainCell = new TerrainCell()
            {
                Coords = new OffsetCoords(0, 0)
            };

            // ACT
            var factTransitionNodes = RegionTransitionHelper.GetNeighborBorderNodes(currentNode,
                currentTerrainCell,
                targetRegionBorders,
                targetTerrainCell);

            // ASSERT
            factTransitionNodes.Should().BeEquivalentTo(expectedTransitionNodes);
        }

        /// <summary>
        /// Тест проверяет корректность обнаружения узлов, доступных для перехода в соседнюю провинцию.
        /// Движение по левой границе вверх.
        /// </summary>
        [Test]
        public void GetNeighborBorderNodes_TopLeftMoveUp_ReturnsBottomLeft()
        {
            // ARRANGE
            const int REGION_SIZE = 20;
            // У нас есть 2 региона, расположенных по соседству.

            var regionSchemeMock = new Mock<ILocationScheme>();
            var regionScheme = regionSchemeMock.Object;

            var currentRegion = new Province(REGION_SIZE);
            FillRegion(REGION_SIZE, regionScheme, currentRegion);

            // Берём правую верхнюю точку для перехода.
            var currentNode = currentRegion.Nodes.OfType<ProvinceNode>().Single(node => node.OffsetX == 0 && node.OffsetY == 0);

            var currentTerrainCell = new TerrainCell()
            {
                Coords = new OffsetCoords(0, 1)
            };

            var targetRegion = new Province(REGION_SIZE);
            FillRegion(REGION_SIZE, regionScheme, targetRegion);
            var expectedTransitionNodes = new[] {
                targetRegion.Nodes.OfType<ProvinceNode>().Single(node => node.OffsetX == 0 && node.OffsetY == REGION_SIZE - 1),
            };

            var targetRegionBorders = targetRegion.Nodes.OfType<ProvinceNode>().Where(node => node.IsBorder);
            var targetTerrainCell = new TerrainCell()
            {
                Coords = new OffsetCoords(0, 0)
            };

            // ACT
            var factTransitionNodes = RegionTransitionHelper.GetNeighborBorderNodes(currentNode,
                currentTerrainCell,
                targetRegionBorders,
                targetTerrainCell);

            // ASSERT
            factTransitionNodes.Should().BeEquivalentTo(expectedTransitionNodes);
        }

        /// <summary>
        /// Тест проверяет корректность обнаружения узлов, доступных для перехода в соседнюю провинцию.
        /// Движение по левой границе вниз.
        /// </summary>
        [Test]
        public void GetNeighborBorderNodes_BottomLeftMoveUp_ReturnsTopLeft()
        {
            // ARRANGE
            const int REGION_SIZE = 20;
            // У нас есть 2 региона, расположенных по соседству.

            var regionSchemeMock = new Mock<ILocationScheme>();
            var regionScheme = regionSchemeMock.Object;

            var currentRegion = new Province(REGION_SIZE);
            FillRegion(REGION_SIZE, regionScheme, currentRegion);

            // Берём правую верхнюю точку для перехода.
            var currentNode = currentRegion.Nodes.OfType<ProvinceNode>().Single(node => node.OffsetX == 0 && node.OffsetY == REGION_SIZE - 1);

            var currentTerrainCell = new TerrainCell()
            {
                Coords = new OffsetCoords(0, 0)
            };

            var targetRegion = new Province(REGION_SIZE);
            FillRegion(REGION_SIZE, regionScheme, targetRegion);
            var expectedTransitionNodes = new[] {
                targetRegion.Nodes.OfType<ProvinceNode>().Single(node => node.OffsetX == 0 && node.OffsetY == 0),
                targetRegion.Nodes.OfType<ProvinceNode>().Single(node => node.OffsetX == 1 && node.OffsetY == 0)
            };

            var targetRegionBorders = targetRegion.Nodes.OfType<ProvinceNode>().Where(node => node.IsBorder);
            var targetTerrainCell = new TerrainCell()
            {
                Coords = new OffsetCoords(0, 1)
            };

            // ACT
            var factTransitionNodes = RegionTransitionHelper.GetNeighborBorderNodes(currentNode,
                currentTerrainCell,
                targetRegionBorders,
                targetTerrainCell);

            // ASSERT
            factTransitionNodes.Should().BeEquivalentTo(expectedTransitionNodes);
        }

        private static void FillRegion(int regionSize, ILocationScheme regionScheme, Province currentRegion)
        {
            for (var x = 0; x < regionSize; x++)
            {
                for (var y = 0; y < regionSize; y++)
                {
                    currentRegion.AddNode(new ProvinceNode(x, y, regionScheme)
                    {
                        IsBorder = x == 0 || x == regionSize - 1 || y == 0 || y == regionSize - 1
                    });
                }
            }
        }
    }
}