using FluentAssertions;
using Moq;

using NUnit.Framework;

using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.Tactics.Tests
{
    [TestFixture]
    public class ActUsageRandomSourceTests
    {
        /// <summary>
        /// Тест проверяет, что при минимальном броске игральной кости будет выбрано минимальное значение эффективности.
        /// </summary>
        [Test]
        public void SelectEfficient_DiceRolls1_ReturnsMin()
        {
            // ARRANGE
            const int minEfficient = 5;
            const int maxEfficient = 10;

            var diceMock = new Mock<IDice>();
            diceMock.Setup(x => x.Roll(It.IsAny<int>())).Returns(1);
            var dice = diceMock.Object;

            var service = new ActUsageRandomSource(dice);



            // ACT
            var factRoll = service.SelectEfficient(minEfficient, maxEfficient);



            // ASSERT
            factRoll.Should().Be(minEfficient);
        }

        /// <summary>
        /// Тест проверяет, что при максимальном броске игральной кости будет выбрано максимальное значение эффективности.
        /// </summary>
        [Test]
        public void SelectEfficient_DiceRollsN_ReturnsMax()
        {
            // ARRANGE
            const int minEfficient = 5;
            const int maxEfficient = 10;

            var diceMock = new Mock<IDice>();
            diceMock.Setup(x => x.Roll(It.IsAny<int>())).Returns<int>(n => n);
            var dice = diceMock.Object;

            var service = new ActUsageRandomSource(dice);



            // ACT
            var factRoll = service.SelectEfficient(minEfficient, maxEfficient);



            // ASSERT
            factRoll.Should().Be(maxEfficient);
        }
    }
}