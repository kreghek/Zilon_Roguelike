using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.MapGenerators.Tests
{
    [TestFixture()]
    public class SectorGeneratorRandomSourceTests
    {
        /// <summary>
        /// Тест проверяет, что источник рандома выбирает комнату даже при самом низком результате броска,
        /// если указана 100 веротяность выбора соседа.
        /// </summary>
        [Test()]
        public void RollConnectedRoomsTest()
        {
            // ARRANGE

            var maxNeighbor = 1;

            var neighborProbably = 100;

            var rollIndex = 0;

            var diceMock = new Mock<IDice>();
            diceMock.Setup(x => x.Roll(It.IsAny<int>())).Returns<int>(n =>
            {
                var result = 0;
                if (rollIndex == 0)
                {
                    // это бросок на проверку вероятности.
                    result = 1;
                }
                else if (rollIndex == 1)
                {
                    // это бросок на выбор комнаты
                    result = 1;
                }


                rollIndex++;

                return result;
            });
            var dice = diceMock.Object;


            var randomSource = new SectorGeneratorRandomSource(dice);

            var currentRoom = new Room();

            var availableRooms = new[] { new Room() };



            // ACT
            var factRooms = randomSource.RollConnectedRooms(currentRoom,
                maxNeighbor,
                neighborProbably,
                availableRooms);



            // ASSERT
            factRooms[0].Should().Be(availableRooms[0]);
        }
    }
}