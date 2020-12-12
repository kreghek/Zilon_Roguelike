using System.Threading.Tasks;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Graphs;
using Zilon.Core.MapGenerators.PrimitiveStyle;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common;

namespace Zilon.Core.Tests.Tactics.Behaviour
{
    /// <summary>
    /// Тест проверяет, что источник намерений генерирует задачу после указания целевого узла.
    /// По окончанию задачи на перемещение должен выдавать пустые задачи.
    /// </summary>
    [TestFixture]
    public class HumanActorTaskSourceTests
    {
        /// <summary>
        /// Тест проверяет получение задачи актёра после указания намерения.
        /// </summary>
        [Test]
        // Ограничение по времени добавлено на случай, если эта тут наступит бесконечное ожидание.
        [Timeout(1000)]
        public async Task GetActorTaskAsync_GetActorTaskAfterIntention_ReturnsActorTask()
        {
            // ARRANGE

            var map = await SquareMapFactory.CreateAsync(10).ConfigureAwait(false);

            var actorNode = map.Nodes.SelectByHexCoords(0, 0);

            var actor = CreateActor(map, actorNode);

            var taskMock = new Mock<IActorTask>();
            var task = taskMock.Object;
            var intentionMock = new Mock<IIntention>();
            intentionMock.Setup(x => x.CreateActorTask(It.IsAny<IActor>())).Returns(task);
            var intention = intentionMock.Object;

            using var taskSource = new HumanActorTaskSource<ISectorTaskSourceContext>();

            var contextMock = new Mock<ISectorTaskSourceContext>();
            var context = contextMock.Object;

            // ACT

            var getActorTaskTask = taskSource.GetActorTaskAsync(actor, context);
            await taskSource.IntentAsync(intention, actor).ConfigureAwait(false);
            var factActorTask = await getActorTaskTask.ConfigureAwait(false);

            // ASSERT
            factActorTask.Should().Be(task);
        }

        /// <summary>
        /// Тест проверяет получение задачи актёра после указания намерения.
        /// </summary>
        [Test]
        // Ограничение по времени добавлено на случай, если эта тут наступит бесконечное ожидание.
        [Timeout(1000)]
        public async Task GetActorTaskAsync_GetActorTaskAfterIntention_ReturnsActorTask2()
        {
            // ARRANGE

            var map = await SquareMapFactory.CreateAsync(10).ConfigureAwait(false);

            var actorNode = map.Nodes.SelectByHexCoords(0, 0);

            var actor = CreateActor(map, actorNode);

            var taskMock = new Mock<IActorTask>();
            var task = taskMock.Object;
            var intentionMock = new Mock<IIntention>();
            intentionMock.Setup(x => x.CreateActorTask(It.IsAny<IActor>())).Returns(task);
            var intention = intentionMock.Object;

            var contextMock = new Mock<ISectorTaskSourceContext>();
            var context = contextMock.Object;

            using var taskSource = new HumanActorTaskSource<ISectorTaskSourceContext>();

            // ACT

            await taskSource.IntentAsync(intention, actor).ConfigureAwait(false);
            var getActorTaskTask = taskSource.GetActorTaskAsync(actor, context);
            var factActorTask = await getActorTaskTask.ConfigureAwait(false);

            // ASSERT
            factActorTask.Should().Be(task);
        }

        private static IActor CreateActor(IMap map, IGraphNode startNode)
        {
            var playerMock = new Mock<IPlayer>();
            var player = playerMock.Object;

            var personMock = new Mock<IPerson>();
            personMock.SetupGet(x => x.PhysicalSize).Returns(PhysicalSizePattern.Size1);
            var person = personMock.Object;

            var taskSourceMock = new Mock<IActorTaskSource<ISectorTaskSourceContext>>();
            var taskSource = taskSourceMock.Object;

            var actor = new Actor(person, taskSource, startNode);

            map.HoldNode(startNode, actor);

            return actor;
        }
    }
}