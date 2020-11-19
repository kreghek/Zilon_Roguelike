using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Common;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Tactics;

namespace Zilon.Core.Tests.Tactics
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class ActUsageRandomSourceTests
    {
        /// <summary>
        /// Тест проверяет, что при минимальном броске игральной кости будет выбрано минимальное
        /// значение эффективности.
        /// </summary>
        [Test]
        public void SelectEfficient_DiceRolls1_ReturnsMin()
        {
            // ARRANGE
            const int diceEdges = 3;
            const int diceCount = 1;
            const int expectedRoll = 1 * diceCount; // потому что один бросок кости. Минимальное значение броска - 1.

            var roll = new Roll(diceEdges, diceCount);

            var diceMock = new Mock<IDice>();
            diceMock.Setup(x => x.Roll(It.IsAny<int>())).Returns(1);
            var dice = diceMock.Object;

            var service = new TacticalActUsageRandomSource(dice);

            // ACT
            var factRoll = service.RollEfficient(roll);

            // ASSERT
            factRoll.Should().Be(expectedRoll);
        }

        /// <summary>
        /// Тест проверяет, что при максимальном броске игральной кости будет выбрано максимальное значение эффективности.
        /// </summary>
        [Test]
        public void SelectEfficient_DiceRollsN_ReturnsMax()
        {
            // ARRANGE
            const int diceEdges = 3;
            const int diceCount = 1;
            const int
                expectedRoll =
                    diceEdges * diceCount; // потому что один бросок кости. Максимальное значение броска - diceEdges.

            var roll = new Roll(diceEdges, diceCount);

            var diceMock = new Mock<IDice>();
            diceMock.Setup(x => x.Roll(It.IsAny<int>())).Returns<int>(n => n);
            var dice = diceMock.Object;

            var service = new TacticalActUsageRandomSource(dice);

            // ACT
            var factRoll = service.RollEfficient(roll);

            // ASSERT
            factRoll.Should().Be(expectedRoll);
        }
    }
}