using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.Tests.CommonServices.Dices
{
    [TestFixture]
    public class IDiceExtensionsTests
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
            var factRoll = IDiceExtensions.Roll(dice, 1, 3);



            // ASSERT
            factRoll.Should().Be(expectedRoll);
        }
    }
}