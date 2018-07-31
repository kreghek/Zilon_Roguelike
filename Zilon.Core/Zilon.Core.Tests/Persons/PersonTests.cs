using FluentAssertions;

using NUnit.Framework;

using Zilon.Core.Common;
using Zilon.Core.Components;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Persons
{
    [TestFixture]
    public class PersonTests
    {
        /// <summary>
        /// Тест проверяет, что персонаж корректно обрабатывает назначение экипировки.
        /// </summary>
        [Test]
        public void SetEquipment_SetSingleEquipment_HasActs()
        {
            // ARRANGE
            var personScheme = new PersonScheme
            {
                SlotCount = 3
            };

            var person = new Person(personScheme);

            var propScheme = new PropScheme
            {
                Equip = new PropEquipSubScheme()
            };

            var tacticalActScheme = new TacticalActScheme
            {
                Stats = new TacticalActStatsSubScheme
                {
                    Efficient = new Range<float>(1, 1),
                },
                Dependency = new[] {
                    new TacticalActDependencySubScheme(CombatStatType.Undefined, 1)
                }
            };

            var equipment = new Equipment(propScheme, new []{ tacticalActScheme });

            const int expectedSlotIndex = 0;



            // ACT

            person.EquipmentCarrier.SetEquipment(equipment, expectedSlotIndex);



            // ARRANGE
            person.TacticalActCarrier.Acts[0].Stats.Should().Be(tacticalActScheme);
        }
    }
}