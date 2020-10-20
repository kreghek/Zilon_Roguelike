using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.MapGenerators;

namespace Zilon.Core.Tests.MapGenerators
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class ChestGeneratorRandomSourceTests
    {
        [Test]
        public void RollChestCount_AlwaysMaxRoll_MaxCountRolled()
        {
            // ARRANGE
            var diceMock = new Mock<IDice>();
            diceMock.Setup(x => x.Roll(It.IsAny<int>())).Returns<int>(n => n);
            var dice = diceMock.Object;

            var random = new ChestGeneratorRandomSource(dice);



            // ACT
            var factRolled = random.RollChestCount(3);



            // ASSERT
            factRolled.Should().Be(3);
        }
    }
}