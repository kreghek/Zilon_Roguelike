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
        /// <summary>
        /// Тест проверяет, что если есть возможность поставить объекты интерьера,
        /// то они будут.
        /// Размер квадрата 4х4, потому что генерится 1 объект интерьера на каждые 4 узла комнаты.
        /// </summary>
        [Test]
        public void RollInteriorObjectsTest()
        {
            const int SQARE_SIZE = 4;

            // ARRANGE

            var diceMock = new Mock<IDice>();
            diceMock.Setup(x => x.Roll(It.IsAny<int>())).Returns(1);
            var dice = diceMock.Object;

            var interiorRandomSource = new InteriorObjectRandomSource(dice);

            var coords = new OffsetCoords[SQARE_SIZE * SQARE_SIZE];
            for (var i = 0; i < SQARE_SIZE; i++)
            {
                for (var j = 0; j < SQARE_SIZE; j++)
                {
                    coords[i + j * SQARE_SIZE] = new OffsetCoords(i, j);
                }
            }

            // ACT
            var factMetas = interiorRandomSource.RollInteriorObjects(coords);

            // ASSERT
            factMetas.Should().NotBeEmpty();
        }
    }
}