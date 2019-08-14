using System.Collections.Generic;

using FluentAssertions;

using Moq;

using NUnit.Framework;
using Zilon.Core.MapGenerators;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tests.Tactics
{
    [TestFixture]
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

            var actorNodeMock = new Mock<IMapNode>();
            var actorNode = actorNodeMock.Object;

            var transition = new RoomTransition("test");

            var testedTrasitions = new Dictionary<IMapNode, RoomTransition>
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

            var actorNodeMock = new Mock<IMapNode>();
            var actorNode = actorNodeMock.Object;

            var transitionNodeMock = new Mock<IMapNode>();
            var transitionNode = transitionNodeMock.Object;

            var transition = new RoomTransition("test");

            var testedTrasitions = new Dictionary<IMapNode, RoomTransition>
            {
                { transitionNode, transition }
            };

            var testedNodes = new[] { actorNode };

            RoomTransition expectedTransition = null;



            // ACT
            var factTransition = TransitionDetection.Detect(testedTrasitions, testedNodes);



            // ASSERT
            factTransition.Should().Be(expectedTransition);
        }
    }
}