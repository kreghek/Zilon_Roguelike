using FluentAssertions;

using LightInject;

using Moq;

using NUnit.Framework;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tests.Commands
{
    [TestFixture()]
    public class NextTurnCommandTests: CommandTestBase
    {
        /// <summary>
        /// Тест проверяет, что можно выполнить выполнить следующий ход.
        /// По идее, следующий ход можно вызвать всегда. То есть CanExecute всегда true.
        /// </summary>
        [Test]
        public void CanExecuteTest()
        {
            // ARRANGE
            var command = _container.GetInstance<NextTurnCommand>();



            // ACT
            var canExecute = command.CanExecute();


            // ASSERT
            canExecute.Should().Be(true);
        }

        /// <summary>
        /// Тест проверяет, что при выполнении команды вызывается обновление состояния сектора.
        /// </summary>
        [Test]
        public void ExecuteTest()
        {
            // ARRANGE
            var command = _container.GetInstance<NextTurnCommand>();
            var humanTaskSourceMock = _container.GetInstance<Mock<IHumanActorTaskSource>>();


            // ACT
            command.Execute();


            // ASSERT
            humanTaskSourceMock.Verify(x => x.Intent(It.IsAny<IIntention>()), Times.Once);
        }

        protected override void RegisterSpecificServices(IMap testMap, Mock<IPlayerState> playerStateMock)
        {
            var decisionSourceMock = new Mock<IDecisionSource>();
            decisionSourceMock.Setup(x => x.SelectIdleDuration(It.IsAny<int>(), It.IsAny<int>())).Returns(1);
            var decisionSource = decisionSourceMock.Object;

            _container.Register(factory => decisionSource, new PerContainerLifetime());
            _container.Register<NextTurnCommand>(new PerContainerLifetime());
        }
    }
}