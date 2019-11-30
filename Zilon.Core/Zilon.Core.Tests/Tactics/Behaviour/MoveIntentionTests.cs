using NUnit.Framework;
using Zilon.Core.Tactics.Behaviour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Zilon.Core.Tactics.Spatial;
using FluentAssertions;
using Zilon.Core.Graphs;

namespace Zilon.Core.Tactics.Behaviour.Tests
{
    [TestFixture][Parallelizable(ParallelScope.All)]
    public class MoveIntentionTests
    {
        /// <summary>
        /// Тест проверяет, что при первом запросе команды у намерения
        /// возвращается задача на перемещение в указанный узел.
        /// </summary>
        [Test]
        public void CreateActorTask_FirstMoveTask_ReturnsMoveTask()
        {
            // ARRANGE
            var mapMock = new Mock<IMap>();
            var map = mapMock.Object;

            var nodeMock = new Mock<IGraphNode>();
            var node = nodeMock.Object;

            var actorMock = new Mock<IActor>();
            var actor = actorMock.Object;

            var intention = new MoveIntention(node, map);



            // ACT
            var factTask = intention.CreateActorTask(null, actor);



            // ASSERT
            factTask.Should().BeOfType<MoveTask>()
                .And.Subject.As<MoveTask>().TargetNode.Should().Be(node);
        }

        /// <summary>
        /// Тест проверяет, что при запросе команды у намерения,
        /// когда текущая команда отличается от перемещения,
        /// возвращается задача на перемещение в указанный узел.
        /// </summary>
        /// <remarks>
        /// Это означает, что если персонажу нужно куда-то переместиться, то текущая задача прерывается.
        /// </remarks>
        [Test]
        public void CreateActorTask_CurrentIsNotMoveTask_ReturnsMoveTask()
        {
            // ARRANGE
            var mapMock = new Mock<IMap>();
            var map = mapMock.Object;

            var nodeMock = new Mock<IGraphNode>();
            var node = nodeMock.Object;

            var actorMock = new Mock<IActor>();
            var actor = actorMock.Object;

            var currentTaskMock = new Mock<IActorTask>();
            var currentTask = currentTaskMock.Object;

            var intention = new MoveIntention(node, map);



            // ACT
            var factTask = intention.CreateActorTask(currentTask, actor);



            // ASSERT
            factTask.Should().BeOfType<MoveTask>()
                .And.Subject.As<MoveTask>().TargetNode.Should().Be(node);
        }

        /// <summary>
        /// Тест проверяет, что при запросе команды, если текущая задача
        /// уже является задачей на перемещение в тот же самый узел,
        /// возвращается эта же задача.
        /// </summary>
        /// <remarks>
        /// Это означает, что если персонаж дважды укажет намерение двигаться в один и тот же узел,
        /// то задача будет создана только первый раз. Соответственно, поиск пути будет произведён
        /// только первый раз.
        /// </remarks>
        [Test]
        public void CreateActorTask_CurrentMoveTaskWithSameTarget_ReturnsCurrentMoveTask()
        {
            // ARRANGE
            var mapMock = new Mock<IMap>();
            var map = mapMock.Object;

            var nodeMock = new Mock<IGraphNode>();
            var node = nodeMock.Object;

            var actorMock = new Mock<IActor>();
            var actor = actorMock.Object;

            var currentTask = new MoveTask(actor, node, map);

            var intention = new MoveIntention(node, map);



            // ACT
            var factTask = intention.CreateActorTask(currentTask, actor);



            // ASSERT
            factTask.Should().Be(currentTask);
        }

        /// <summary>
        /// Тест проверяет, что при запросе команды, если текущая задача
        /// уже является задачей на перемещение, но в другой целевой узел,
        /// будет возвращена новая задача с новым целевым узлом.
        /// </summary>
        [Test]
        public void CreateActorTask_CurrentMoveTaskWithOtherTarget_ReturnsNewMoveTask()
        {
            // ARRANGE
            var mapMock = new Mock<IMap>();
            var map = mapMock.Object;

            var nodeMock = new Mock<IGraphNode>();
            var node = nodeMock.Object;

            var node2Mock = new Mock<IGraphNode>();
            var node2 = nodeMock.Object;

            var actorMock = new Mock<IActor>();
            var actor = actorMock.Object;

            var currentTask = new MoveTask(actor, node, map);

            var intention = new MoveIntention(node2, map);



            // ACT
            var factTask = intention.CreateActorTask(currentTask, actor);



            // ASSERT
            factTask.Should().BeOfType<MoveTask>()
                .And.Subject.As<MoveTask>().TargetNode.Should().Be(node2);
        }
    }
}