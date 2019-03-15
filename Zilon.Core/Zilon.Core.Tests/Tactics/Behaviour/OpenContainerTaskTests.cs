using System;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task Execute_ValidLength_ActorOpenedContainerAsync()
        {
            // ARRANGE
            var map = await SquareMapFactory.CreateAsync(10);

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
        /// Тест проверяет, что задача выполняет проверку доступности сундука.
        /// </summary>
        [Test]
        public void Execute_CheckWalls_MapTargetIsOnLineInvoked()
        {
            // ARRANGE
            var mapMock = new Mock<ISectorMap>();
            mapMock.Setup(x => x.TargetIsOnLine(It.IsAny<IMapNode>(), It.IsAny<IMapNode>()))
                .Returns(true);
            var map = mapMock.Object;

            var actorNodeMock = new Mock<IMapNode>();
            var actorNode = actorNodeMock.Object;

            var actorMock = new Mock<IActor>();
            actorMock.SetupGet(x => x.Node).Returns(actorNode);
            var actor = actorMock.Object;

            var containerNodeMock = new Mock<IMapNode>();
            var containerNode = containerNodeMock.Object;

            var container = CreateContainer(containerNode);

            var method = CreateMethod();

            var task = new OpenContainerTask(actor, container, method, map);



            // ACT
            task.Execute();



            // ASSERT
            mapMock.Verify(x => x.TargetIsOnLine(
                It.Is<IMapNode>(n => n == actorNode),
                It.Is<IMapNode>(n => n == containerNode))
                );
        }

        private IOpenContainerMethod CreateMethod()
        {
            var methodMock = new Mock<IOpenContainerMethod>();
            var method = methodMock.Object;
            return method;
        }

        private IPropContainer CreateContainer(IMapNode containerNode)
        {
            var containerMock = new Mock<IPropContainer>();
            containerMock.SetupGet(x => x.Node).Returns(containerNode);
            var container = containerMock.Object;
            return container;
        }
    }
}