using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Persons;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Persons
{
    [TestFixture]
    public class PersonTests
    {
        [Test]
        public void PersonTest()
        {
            // ARRANGE
            var person = new Person();

            var scheme = new PropScheme
            {
                Equip = new PropEquipSubScheme()
            };

            var tacticActMock = new Mock<ITacticalAct>();
            var tacticAct = tacticActMock.Object;

            var equipment = new Equipment(scheme, new []{ tacticAct });

            const int expectedSlotIndex = 0;



            // ACT

            person.EquipmentCarrier.SetEquipment(equipment, expectedSlotIndex);



            // ARRANGE
            person.Acts[0].Should().Be(tacticAct);
        }
    }
}