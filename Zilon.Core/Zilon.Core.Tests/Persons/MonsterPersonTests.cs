using System;

using FluentAssertions;

using NUnit.Framework;

using Zilon.Core.Common;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Persons
{
    [TestFixture]
    public class MonsterPersonTests
    {
        /// <summary>
        /// Тест проверяет, что нет исключений при создании монстра.
        /// </summary>
        [Test]
        public void Constructor_DefaultParams_NoException()
        {
            // ARRANGE
            var monsterScheme = new MonsterScheme
            {
                PrimaryAct = new TacticalActStatsSubScheme
                {
                    Efficient = new Range<float>(1, 2)
                }
            };

            // ACT
            Action act = () =>
            {
                var monster = new MonsterPerson(monsterScheme);
            };



            // ARRANGE
            act.Should().NotThrow();
        }

        /// <summary>
        /// Тест проверяет, что монстру корректно присвается значение Hp.
        /// </summary>
        [Test]
        public void Constructor_HpInScheme_ActorHpEqualsSchemeHp()
        {
            // ARRANGE
            const int expectedHp = 100;
            var monsterScheme = new MonsterScheme
            {
                Hp = expectedHp,
                PrimaryAct = new TacticalActStatsSubScheme
                {
                    Efficient = new Range<float>(1, 2)
                }
            };

            // ACT
            var monster = new MonsterPerson(monsterScheme);




            // ARRANGE
            monster.Hp.Should().Be(expectedHp);
        }
    }
}