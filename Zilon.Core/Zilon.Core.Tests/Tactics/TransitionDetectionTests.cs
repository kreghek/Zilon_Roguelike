using System.Collections.Generic;

using Zilon.Core.Graphs;
using Zilon.Core.MapGenerators;
using Zilon.Core.Tactics;
using Zilon.Core.World;

namespace Zilon.Core.Tests.Tactics
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class TransitionDetectionTests
    {
        /// <summary>
        /// Проверяет, что если указанный узел попадает в единственный переход, то будет
        /// возвращён этот переход.
        /// </summary>
        [Test]
        public void DetectTest_OneInSingleTransition_ReturnsThisTransition()
        {
            // ARRANGE

            var actorNodeMock = new Mock<IGraphNode>();
            var actorNode = actorNodeMock.Object;

            var sectorNodeMock = new Mock<ISectorNode>();
            var sectorNode = sectorNodeMock.Object;

            var transition = new RoomTransition(sectorNode);

            var testedTrasitions = new Dictionary<IGraphNode, RoomTransition>
            {
                {
                    actorNode, transition
                }
            };

            var testedNodes = new[]
            {
                actorNode
            };

            var expectedTransition = transition;

            // ACT
            var factTransition = TransitionDetection.Detect(testedTrasitions, testedNodes);

            // ASSERT
            factTransition.Should().Be(expectedTransition);
        }

        /// <summary>
        /// Проверяет, что если указанный узел попадает в единственный переход, то будет
        /// возвращён этот переход.
        /// </summary>
        [Test]
        public void DetectTest_OneInOutOfSingleTransition_ReturnsNull()
        {
            // ARRANGE

            var actorNodeMock = new Mock<IGraphNode>();
            var actorNode = actorNodeMock.Object;

            var transitionNodeMock = new Mock<IGraphNode>();
            var transitionNode = transitionNodeMock.Object;

            var sectorNodeMock = new Mock<ISectorNode>();
            var sectorNode = sectorNodeMock.Object;

            var transition = new RoomTransition(sectorNode);

            var testedTrasitions = new Dictionary<IGraphNode, RoomTransition>
            {
                {
                    transitionNode, transition
                }
            };

            var testedNodes = new[]
            {
                actorNode
            };

            RoomTransition expectedTransition = null;

            // ACT
            var factTransition = TransitionDetection.Detect(testedTrasitions, testedNodes);

            // ASSERT
            factTransition.Should().Be(expectedTransition);
        }
    }
}