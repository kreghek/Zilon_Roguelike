using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tests.Commands
{
    [TestFixture]
    public class NextTurnCommandTests : CommandTestBase
    {
        /// <summary>
        /// Тест проверяет, что можно выполнить выполнить следующий ход.
        /// По идее, следующий ход можно вызвать всегда. То есть CanExecute всегда true.
        /// </summary>
        [Test]
        public void CanExecuteTest()
        {
            // ARRANGE
            var command = ServiceProvider.GetRequiredService<NextTurnCommand>();

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
            var command = ServiceProvider.GetRequiredService<NextTurnCommand>();
            var humanTaskSourceMock =
                ServiceProvider.GetRequiredService<Mock<IHumanActorTaskSource<ISectorTaskSourceContext>>>();

            // ACT
            command.Execute();

            // ASSERT
            humanTaskSourceMock.Verify(x => x.Intent(It.IsAny<IIntention>(), It.IsAny<IActor>()), Times.Once);
        }

        protected override void RegisterSpecificServices(IMap testMap, Mock<ISectorUiState> playerStateMock)
        {
            var decisionSourceMock = new Mock<IDecisionSource>();
            decisionSourceMock.Setup(x => x.SelectIdleDuration(It.IsAny<int>(), It.IsAny<int>())).Returns(1);
            var decisionSource = decisionSourceMock.Object;

            Container.AddSingleton(factory => decisionSource);
            Container.AddSingleton<NextTurnCommand>();
        }
    }
}