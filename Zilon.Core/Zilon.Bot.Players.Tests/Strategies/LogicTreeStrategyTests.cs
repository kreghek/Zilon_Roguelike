using NUnit.Framework;
using Zilon.Bot.Players.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Zilon.Core.Tactics;
using FluentAssertions;
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
            var logicStateMock = new Mock<ILogicState>();

            var actorTaskMock = new Mock<IActorTask>();
            var actorTask = actorTaskMock.Object;
            logicStateMock.Setup(x => x.GetTask(It.IsAny<IActor>(), It.IsAny<ILogicStateData>()))
                .Returns(actorTask);
            var logicState = logicStateMock.Object;
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
    }
}