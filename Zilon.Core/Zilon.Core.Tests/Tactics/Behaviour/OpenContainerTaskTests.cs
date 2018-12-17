using System;
using System.Linq;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.MapGenerators.PrimitiveStyle;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common;

namespace Zilon.Core.Tests.Tactics.Behaviour
{
    [TestFixture]
    public class OpenContainerTaskTests
    {
        /// <summary>
        /// Тест проверяет, что при расстоянии до контейнера в 1 клетку задача вызывает метод актёра
        /// на открытие контейнера.
        /// </summary>
        [Test]
        public void Execute_ValidLength_ActorOpenedContainer()
        {
            // ARRANGE
            var map = SquareMapFactory.Create(10);

            var actorNode = map.Nodes.Cast<HexNode>().SelectBy(0, 0);

            var actorMock = new Mock<IActor>();
            actorMock.SetupGet(x => x.Node).Returns(actorNode);
            var actor = actorMock.Object;

            var containerNode = map.Nodes.Cast<HexNode>().SelectBy(1, 0);

            var container = CreateContainer(containerNode);

            var method = CreateMethod();

            var task = new OpenContainerTask(actor, container, method, map);



            // ACT
            task.Execute();



            // ASSERT
            actorMock.Verify(x => x.OpenContainer(It.IsAny<IPropContainer>(), It.IsAny<IOpenContainerMethod>()));
        }

        /// <summary>
        /// Тест проверяет, что через стену нельзя открывать сундуки.
        /// </summary>
        [Test]
        public void Execute_Wall_Exception()
        {
            // ARRANGE
            var map = SquareMapFactory.Create(10);
            map.RemoveEdge(0, 0, 1, 0);

            var actorNode = map.Nodes.Cast<HexNode>().SelectBy(0, 0);

            var actorMock = new Mock<IActor>();
            actorMock.SetupGet(x => x.Node).Returns(actorNode);
            var actor = actorMock.Object;

            var containerNode = map.Nodes.Cast<HexNode>().SelectBy(1, 0);

            var container = CreateContainer(containerNode);

            var method = CreateMethod();

            var task = new OpenContainerTask(actor, container, method, map);



            // ACT
            Action act = () => {
                task.Execute();
            };



            // ASSERT
            act.Should().Throw<InvalidOperationException>();
        }

        private IOpenContainerMethod CreateMethod()
        {
            var methodMock = new Mock<IOpenContainerMethod>();
            var method = methodMock.Object;
            return method;
        }

        private IPropContainer CreateContainer(HexNode containerNode)
        {
            var containerMock = new Mock<IPropContainer>();
            containerMock.SetupGet(x => x.Node).Returns(containerNode);
            var container = containerMock.Object;
            return container;
        }
    }
}