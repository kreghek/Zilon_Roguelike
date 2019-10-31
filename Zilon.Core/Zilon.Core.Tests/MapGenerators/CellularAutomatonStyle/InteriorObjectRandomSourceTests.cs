using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.MapGenerators.CellularAutomatonStyle.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class InteriorObjectRandomSourceTests
    {
        [Test]
        public void RollInteriorObjectsTest()
        {
            // ARRANGE

            var diceMock = new Mock<IDice>();
            diceMock.Setup(x => x.Roll(It.IsAny<int>())).Returns(1);
            var dice = diceMock.Object;

            var interiorRandomSource = new InteriorObjectRandomSource(dice);

            var coords = new OffsetCoords[9];
            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    coords[i + j * 3] = new OffsetCoords(i, j);
                }
            }

            // ACT
            var factMetas = interiorRandomSource.RollInteriorObjects(coords);

            // ASSERT
            factMetas.Should().NotBeEmpty();
        }
    }
}