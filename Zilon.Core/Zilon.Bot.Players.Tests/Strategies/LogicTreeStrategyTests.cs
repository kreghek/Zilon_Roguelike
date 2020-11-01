using System;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players.Strategies.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class LogicTreeStrategyTests
    {
        /// <summary>
        ///     Тест проверяет, что стартовое состояние без переходов возвращает задачу.
        /// </summary>
        [Test]
        public void GetActorTask_OneStateWithoutTransitions_ReturnsTask()
        {
            // ARRAGE
            LogicStateTree logicTree = new LogicStateTree();

            CreateLogicState(out ILogicState logicState, out IActorTask actorTask);

            logicTree.Transitions.Add(logicState, Array.Empty<LogicTransition>());
            logicTree.StartState = logicState;

            var actorMock = new Mock<IActor>();
            var actor = actorMock.Object;

            var taskContextMock = new Mock<ISectorTaskSourceContext>();
            var taskContext = taskContextMock.Object;

            LogicTreeStrategy strategy = new LogicTreeStrategy(actor, logicTree);

            // ACT
            IActorTask factTask = strategy.GetActorTask(taskContext);

            // ASSERT
            factTask.Should().Be(actorTask);
        }

        /// <summary>
        ///     Тест проверяет, что если из стартового состояния есть переход на другую стадию, то будет
        ///     возвращена задача следующей стадии.
        /// </summary>
        [Test]
        public void GetActorTask_HasActiveTransitionFromStart_ReturnsTaskOfSecondState()
        {
            // ARRAGE
            LogicStateTree logicTree = new LogicStateTree();

            // Настройка стартового состояния
            CreateLogicState(out ILogicState startLogicState, out IActorTask startActorTask);
            logicTree.StartState = startLogicState;

            // Настройка второго состояния
            CreateLogicState(out ILogicState secondLogicState, out IActorTask secondActorTask);

            var triggerMock = new Mock<ILogicStateTrigger>();
            triggerMock.Setup(x => x.Test(It.IsAny<IActor>(), It.IsAny<ISectorTaskSourceContext>(),
                    It.IsAny<ILogicState>(), It.IsAny<ILogicStrategyData>()))
                .Returns(true);
            var trigger = triggerMock.Object;

            logicTree.Transitions.Add(startLogicState, new[] {new LogicTransition(trigger, secondLogicState)});

            logicTree.Transitions.Add(secondLogicState, new[] {new LogicTransition(trigger, startLogicState)});

            var actorMock = new Mock<IActor>();
            var actor = actorMock.Object;

            var taskContextMock = new Mock<ISectorTaskSourceContext>();
            var taskContext = taskContextMock.Object;

            LogicTreeStrategy strategy = new LogicTreeStrategy(actor, logicTree);

            // ACT
            IActorTask factTask = strategy.GetActorTask(taskContext);

            // ASSERT
            factTask.Should().Be(secondActorTask);
        }

        private static void CreateLogicState(out ILogicState logicState, out IActorTask actorTask)
        {
            var startLogicStateMock = new Mock<ILogicState>();
            var startActorTaskMock = new Mock<IActorTask>();
            actorTask = startActorTaskMock.Object;
            startLogicStateMock.Setup(x => x.GetTask(It.IsAny<IActor>(), It.IsAny<ISectorTaskSourceContext>(),
                    It.IsAny<ILogicStrategyData>()))
                .Returns(actorTask);
            logicState = startLogicStateMock.Object;
        }
    }
}