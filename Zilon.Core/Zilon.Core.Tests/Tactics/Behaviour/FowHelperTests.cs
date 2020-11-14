using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Zilon.Core.Graphs;
using Zilon.Core.Tactics.Spatial;
using Zilon.CoreTestsTemp.Tactics.Behaviour.TestCases;

namespace Zilon.Core.Tactics.Behaviour.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class FowHelperTests
    {
        /// <summary>
        /// Тест проверяет, что на пустой карте данные тумана войны не пустые.
        /// </summary>
        [Test]
        [TestCaseSource(typeof(FowHelperTestCaseDataSource), nameof(FowHelperTestCaseDataSource.TestCases))]
        public void UpdateFowData_SquareMap_ObseringNodesIsNotEmpty(int mapSize, int baseX, int baseY, int radius)
        {
            // ARRANGE

            var map = new SectorHexMap(1000);
            for (var i = 0; i < mapSize; i++)
            {
                for (var j = 0; j < mapSize; j++)
                {
                    var hexNode = new HexNode(i, j);
                    map.AddNode(hexNode);
                }
            }

            var nodeList = new List<SectorMapFowNode>();
            var fowDataMock = new Mock<ISectorFowData>();
            fowDataMock.Setup(x => x.AddNodes(It.IsAny<IEnumerable<SectorMapFowNode>>()))
                .Callback<IEnumerable<SectorMapFowNode>>(nodes => nodeList.AddRange(nodes));
            fowDataMock.Setup(x => x.ChangeNodeState(It.IsAny<SectorMapFowNode>(), It.IsAny<SectorMapNodeFowState>()))
                .Callback<SectorMapFowNode, SectorMapNodeFowState>((fowNode, targetState) =>
                    fowNode.ChangeState(targetState));
            var fowData = fowDataMock.Object;

            var fowContextMock = new Mock<IFowContext>();
            fowContextMock.Setup(x => x.IsTargetVisible(It.IsAny<IGraphNode>(), It.IsAny<IGraphNode>()))
                .Returns<IGraphNode, IGraphNode>((baseNode, targetNode) => map.TargetIsOnLine(baseNode, targetNode));
            fowContextMock.Setup(x => x.GetNext(It.IsAny<IGraphNode>()))
                .Returns<IGraphNode>(node => map.GetNext(node));
            var fowContext = fowContextMock.Object;

            var baseNode = map.HexNodes.Single(x => x.OffsetCoords.CompsEqual(baseX, baseY));

            var expectedObservingNodes = map.HexNodes.Where(x => map.DistanceBetween(x, baseNode) <= radius).ToArray();

            // ACT

            FowHelper.UpdateFowData(fowData, fowContext, baseNode, radius);

            // ARRANGE
            var factObservingNodes = nodeList.Where(x => x.State == SectorMapNodeFowState.Observing).Select(x => x.Node)
                .ToArray();
            factObservingNodes.Should().BeEquivalentTo(expectedObservingNodes);
        }

        /// <summary>
        /// Тест проверяет, что на пустой карте данные тумана войны не пустые.
        /// </summary>
        [Test]
        [TestCaseSource(typeof(FowHelperTestCaseDataSource), nameof(FowHelperTestCaseDataSource.TestCases))]
        public void UpdateFowData_RealHumanSectorFowData_ObseringNodesIsNotEmpty(int mapSize, int baseX, int baseY,
            int radius)
        {
            // ARRANGE

            var map = new SectorHexMap(1000);
            for (var i = 0; i < mapSize; i++)
            {
                for (var j = 0; j < mapSize; j++)
                {
                    var hexNode = new HexNode(i, j);
                    map.AddNode(hexNode);
                }
            }

            var fowData = new HumanSectorFowData();

            var baseNode = map.HexNodes.Single(x => x.OffsetCoords.CompsEqual(baseX, baseY));

            var fowContextMock = new Mock<IFowContext>();
            fowContextMock.Setup(x => x.IsTargetVisible(It.IsAny<IGraphNode>(), It.IsAny<IGraphNode>()))
                .Returns<IGraphNode, IGraphNode>((baseNode, targetNode) => map.TargetIsOnLine(baseNode, targetNode));
            fowContextMock.Setup(x => x.GetNext(It.IsAny<IGraphNode>()))
                .Returns<IGraphNode>(node => map.GetNext(node));
            var fowContext = fowContextMock.Object;

            var expectedObservingNodes = map.HexNodes.Where(x => map.DistanceBetween(x, baseNode) <= radius).ToArray();

            // ACT

            FowHelper.UpdateFowData(fowData, fowContext, baseNode, radius);

            // ARRANGE
            var factObservingNodes = fowData.GetFowNodeByState(SectorMapNodeFowState.Observing).Select(x => x.Node);
            factObservingNodes.Should().BeEquivalentTo(expectedObservingNodes);
        }
    }
}