using System.Threading.Tasks;

using Zilon.Core.Graphs;
using Zilon.Core.MapGenerators.PrimitiveStyle;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tests.Tactics.Behaviour
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class OpenContainerTaskTests
    {
        /// <summary>
        /// Тест проверяет, что задача выполняет проверку доступности сундука.
        /// </summary>
        [Test]
        public void Execute_CheckWalls_MapTargetIsOnLineInvoked()
        {
            // ARRANGE
            var mapMock = new Mock<ISectorMap>();
            mapMock.Setup(x => x.TargetIsOnLine(It.IsAny<IGraphNode>(), It.IsAny<IGraphNode>()))
                .Returns(true);
            var map = mapMock.Object;

            var actorNodeMock = new Mock<IGraphNode>();
            var actorNode = actorNodeMock.Object;

            var actorMock = new Mock<IActor>();
            actorMock.SetupGet(x => x.Node).Returns(actorNode);
            var actor = actorMock.Object;

            var containerNodeMock = new Mock<IGraphNode>();
            var containerNode = containerNodeMock.Object;

            var container = CreateContainer(containerNode);

            var method = CreateMethod();

            var sectorMock = new Mock<ISector>();
            sectorMock.SetupGet(x => x.Map).Returns(map);
            var sector = sectorMock.Object;

            var contextMock = new Mock<IActorTaskContext>();
            contextMock.SetupGet(x => x.Sector).Returns(sector);
            var context = contextMock.Object;

            var task = new OpenContainerTask(actor, context, container, method);

            // ACT
            task.Execute();

            // ASSERT
            mapMock.Verify(x => x.TargetIsOnLine(It.Is<IGraphNode>(n => n == actorNode),
                It.Is<IGraphNode>(n => n == containerNode)));
        }

        /// <summary>
        /// Тест проверяет, что при расстоянии до контейнера в 1 клетку задача вызывает метод актёра
        /// на открытие контейнера.
        /// </summary>
        [Test]
        public async Task Execute_ValidLength_ActorOpenedContainerAsync()
        {
            // ARRANGE
            var map = await SquareMapFactory.CreateAsync(10).ConfigureAwait(false);

            var actorNode = map.Nodes.SelectByHexCoords(0, 0);

            var actorMock = new Mock<IActor>();
            actorMock.SetupGet(x => x.Node).Returns(actorNode);
            var actor = actorMock.Object;

            var containerNode = map.Nodes.SelectByHexCoords(1, 0);

            var container = CreateContainer(containerNode);

            var method = CreateMethod();

            var sectorMock = new Mock<ISector>();
            sectorMock.SetupGet(x => x.Map).Returns(map);
            var sector = sectorMock.Object;

            var contextMock = new Mock<IActorTaskContext>();
            contextMock.SetupGet(x => x.Sector).Returns(sector);
            var context = contextMock.Object;

            var task = new OpenContainerTask(actor, context, container, method);

            // ACT
            task.Execute();

            // ASSERT
            actorMock.Verify(x => x.OpenContainer(It.IsAny<IStaticObject>(), It.IsAny<IOpenContainerMethod>()));
        }

        private static IStaticObject CreateContainer(IGraphNode containerNode)
        {
            var containerMock = new Mock<IStaticObject>();
            containerMock.SetupGet(x => x.Node).Returns(containerNode);
            var container = containerMock.Object;
            return container;
        }

        private IOpenContainerMethod CreateMethod()
        {
            var methodMock = new Mock<IOpenContainerMethod>();
            var method = methodMock.Object;
            return method;
        }
    }
}