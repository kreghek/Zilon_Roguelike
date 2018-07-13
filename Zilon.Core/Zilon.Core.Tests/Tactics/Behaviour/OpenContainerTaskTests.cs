using Moq;

using NUnit.Framework;

using System.Linq;

using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.TestCommon;

namespace Zilon.Core.Tactics.Behaviour.Tests
{
    [TestFixture()]
    public class OpenContainerTaskTests
    {
        /// <summary>
        /// Тест проверяет, что при расстоянии до контейнера в 1 клетку задача вызывает метод актёра
        /// на открытие контейнера.
        /// </summary>
        [Test()]
        public void Execute_ValidLength_ActorOpenedContainer()
        {
            // ARRANGE
            var map = new TestGridGenMap();

            var actorNode = map.Nodes.Cast<HexNode>().SelectBy(0, 0);

            var actorMock = new Mock<IActor>();
            actorMock.SetupGet(x => x.Node).Returns(actorNode);
            var actor = actorMock.Object;

            var containerNode = map.Nodes.Cast<HexNode>().SelectBy(1, 0);

            var container = CreateContainer(containerNode);

            var method = CreateMethod();

            var task = new OpenContainerTask(actor, container, method);



            // ACT
            task.Execute();



            // ASSERT
            actorMock.Verify(x => x.OpenContainer(It.IsAny<IPropContainer>(), It.IsAny<IOpenContainerMethod>()));
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