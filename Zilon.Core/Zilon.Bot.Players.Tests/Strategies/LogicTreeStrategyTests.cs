using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players.Strategies.Tests
{
    [TestFixture]
    public class LogicTreeStrategyTests
    {
        /// <summary>
        /// Тест проверяет, что стартовое состояние без переходов возвращает задачу.
        /// </summary>
        [Test]
        public void GetActorTask_OneStateWithoutTransitions_ReturnsTask()
        {
            // ARRAGE
            var logicTree = new LogicStateTree();

            CreateLogicState(out var logicState, out var actorTask);

            logicTree.Transitions.Add(logicState, new LogicTransition[0]);
            logicTree.StartState = logicState;

            var actorMock = new Mock<IActor>();
            var actor = actorMock.Object;

            var strategy = new LogicTreeStrategy(actor, logicTree);



            // ACT
            var factTask = strategy.GetActorTask();



            // ASSERT
            factTask.Should().Be(actorTask);
        }

        /// <summary>
        /// Тест проверяет, что если из стартового состояния есть переход на другую стадию, то будет
        /// возвращена задача следующей стадии.
        /// </summary>
        [Test]
        public void GetActorTask_HasActiveTransitionFromStart_ReturnsTaskOfSecondState()
        {
            // ARRAGE
            var logicTree = new LogicStateTree();

            // Настройка стартового состояния
            CreateLogicState(out var startLogicState, out var startActorTask);
            logicTree.StartState = startLogicState;

            // Настройка второго состояния
            CreateLogicState(out var secondLogicState, out var secondActorTask);

            var triggerMock = new Mock<ILogicStateTrigger>();
            triggerMock.Setup(x => x.Test(It.IsAny<IActor>(), It.IsAny<ILogicState>(), It.IsAny<ILogicStateData>()))
                .Returns(true);
            var trigger = triggerMock.Object;

            logicTree.Transitions.Add(startLogicState, new LogicTransition[]
            {
                new LogicTransition(trigger, secondLogicState)
            });

            var actorMock = new Mock<IActor>();
            var actor = actorMock.Object;

            var strategy = new LogicTreeStrategy(actor, logicTree);



            // ACT
            var factTask = strategy.GetActorTask();



            // ASSERT
            factTask.Should().Be(secondActorTask);
        }

        private static void CreateLogicState(out ILogicState logicState, out IActorTask actorTask)
        {
            var startLogicStateMock = new Mock<ILogicState>();
            var startActorTaskMock = new Mock<IActorTask>();
            actorTask = startActorTaskMock.Object;
            startLogicStateMock.Setup(x => x.GetTask(It.IsAny<IActor>(), It.IsAny<ILogicStateData>()))
                .Returns(actorTask);
            logicState = startLogicStateMock.Object;
        }
    }
}