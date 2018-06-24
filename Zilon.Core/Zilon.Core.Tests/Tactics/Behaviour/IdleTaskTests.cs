using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Tactics.Behaviour.Bots;

namespace Zilon.Core.Tactics.Behaviour.Tests
{
    [TestFixture]
    public class IdleTaskTests
    {
        /// <summary>
        /// Тест проверяет, что задача на простой на 1 итерацию заканчивается сразу после первой итерации.
        /// </summary>
        [Test]
        public void ExecuteTest()
        {
            // ARRANGE

            var actorMock = new Mock<IActor>();
            var actor = actorMock.Object;

            var decisionSourceMock = new Mock<IDecisionSource>();
            decisionSourceMock.Setup(x => x.SelectIdleDuration(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(1);
            var decisionSource = decisionSourceMock.Object;

            var task = new IdleTask(actor, decisionSource);



            // ACT
            task.Execute();



            // ASSERT
            task.IsComplete.Should().Be(true);
        }
    }
}