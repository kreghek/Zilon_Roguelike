using FluentAssertions;
using Moq;
using NUnit.Framework;
using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.Tests.CommonServices.Dices
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class DiceExtensionsTests
    {
        /// <summary>
        ///  Тест проверяет, что при минимальном броске будет минимальное значение диапазона.
        /// </summary>
        [Test]
        public void Roll_1to3_Returns1()
        {
            // ARRANGE

            const int expectedRoll = 1;

            var diceMock = new Mock<IDice>();
            diceMock.Setup(x => x.Roll(It.IsAny<int>())).Returns(expectedRoll);
            var dice = diceMock.Object;


            // ACT
            var factRoll = dice.Roll(1, 3);


            // ASSERT
            factRoll.Should().Be(expectedRoll);
        }

        /// <summary>
        ///  Тест проверяет, что при максимальном броске будет максимальное значение диапазона.
        /// </summary>
        [Test]
        public void Roll_1to3_Returns3()
        {
            // ARRANGE

            const int expectedRoll = 3;

            var diceMock = new Mock<IDice>();
            // Выбрасываем максимальное значение
            diceMock.Setup(x => x.Roll(It.IsAny<int>())).Returns<int>(n => n);
            var dice = diceMock.Object;


            // ACT
            var factRoll = dice.Roll(1, 3);


            // ASSERT
            factRoll.Should().Be(expectedRoll);
        }

        /// <summary>
        ///  Тест проверяет, что при максимальном броске будет максимальное значение диапазона.
        /// </summary>
        [Test]
        public void Roll_5to10_Returns10()
        {
            // ARRANGE

            const int expectedRoll = 10;

            var diceMock = new Mock<IDice>();
            diceMock.Setup(x => x.Roll(It.IsAny<int>())).Returns<int>(n => n);
            var dice = diceMock.Object;


            // ACT
            var factRoll = dice.Roll(5, 10);


            // ASSERT
            factRoll.Should().Be(expectedRoll);
        }

        /// <summary>
        ///  Тест проверяет, что при одинаковых мин и макс корректно возвращается мин.
        /// </summary>
        [Test]
        public void Roll_5to5_Returns5()
        {
            // ARRANGE

            const int expectedRoll = 5;

            var diceMock = new Mock<IDice>();
            diceMock.Setup(x => x.Roll(It.IsAny<int>())).Returns<int>(n => n);
            var dice = diceMock.Object;


            // ACT
            var factRoll = dice.Roll(5, 5);


            // ASSERT
            factRoll.Should().Be(expectedRoll);
        }
    }
}