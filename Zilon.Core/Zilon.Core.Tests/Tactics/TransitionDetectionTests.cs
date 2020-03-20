using System.Collections.Generic;

using FluentAssertions;

using Moq;

using NUnit.Framework;
using Zilon.Core.Graphs;
using Zilon.Core.Tactics;

namespace Zilon.Core.Tests.Tactics
{
    [TestFixture][Parallelizable(ParallelScope.All)]
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

            var transition = new SectorTransition("test");

            var testedTrasitions = new Dictionary<IGraphNode, SectorTransition>
            {
                { actorNode, transition }
            };

            var testedNodes = new[] { actorNode };

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

            var transition = new SectorTransition("test");

            var testedTrasitions = new Dictionary<IGraphNode, SectorTransition>
            {
                { transitionNode, transition }
            };

            var testedNodes = new[] { actorNode };

            SectorTransition expectedTransition = null;



            // ACT
            var factTransition = TransitionDetection.Detect(testedTrasitions, testedNodes);



            // ASSERT
            factTransition.Should().Be(expectedTransition);
        }
    }
}