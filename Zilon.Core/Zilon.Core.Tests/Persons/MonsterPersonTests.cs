using System;

using FluentAssertions;

using NUnit.Framework;

using Zilon.Core.Persons;
using Zilon.Core.Tests.Common.Schemes;

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
            var monsterScheme = new TestMonsterScheme
            {
                PrimaryAct = new TestTacticalActStatsSubScheme()
            };

            // ACT
            Action act = () =>
            {
                // ReSharper disable once UnusedVariable
                var monster = new MonsterPerson(monsterScheme);
            };



            // ARRANGE
            act.Should().NotThrow();
        }

        /// <summary>
        /// Тест проверяет, что монстру корректно присваивается значение Hp.
        /// </summary>
        [Test]
        public void Constructor_HpInScheme_ActorHpEqualsSchemeHp()
        {
            // ARRANGE
            const int expectedHp = 100;
            var monsterScheme = new TestMonsterScheme
            {
                Hp = expectedHp,
                PrimaryAct = new TestTacticalActStatsSubScheme()
            };

            // ACT
            var monster = new MonsterPerson(monsterScheme);




            // ARRANGE
            monster.Hp.Should().Be(expectedHp);
        }

        /// <summary>
        /// Тест проверяет, что для монстров выбрасывается сообщение на не поддерживаемые компоненты (Развитие).
        /// </summary>
        [Test]
        public void EvolutionData_ThrowNotSupported()
        {
            // ARRANGE
            var monster = CreateMonster();

            // ReSharper disable once ConvertToLocalFunction
            Action<MonsterPerson> requestPropertyAct = m =>
            {
                // ReSharper disable once UnusedVariable
                var tmp = m.EvolutionData;
            };

            //ACT
            var act = ActUnsupportedMonsterComponent(monster, requestPropertyAct);


            // ASSERT
            UnsupportedMonsterComponent(act);
        }

        /// <summary>
        /// Тест проверяет, что для монстров выбрасывается сообщение на неподдерживаемые компоненты (Инвентарь).
        /// </summary>
        [Test]
        public void Inventory_ThrowNotSupported()
        {
            // ARRANGE
            var monster = CreateMonster();

            // ReSharper disable once ConvertToLocalFunction
            Action<MonsterPerson> requestPropertyAct = m =>
            {
                // ReSharper disable once UnusedVariable
                var tmp = m.Inventory;
            };

            //ACT
            var act = ActUnsupportedMonsterComponent(monster, requestPropertyAct);


            // ASSERT
            UnsupportedMonsterComponent(act);
        }

        private static MonsterPerson CreateMonster()
        {
            var monsterScheme = new TestMonsterScheme
            {
                PrimaryAct = new TestTacticalActStatsSubScheme()
            };
            var monster = new MonsterPerson(monsterScheme);
            return monster;
        }

        private Action ActUnsupportedMonsterComponent(MonsterPerson monster, Action<MonsterPerson> requestPropertyAct)
        {
            // ReSharper disable once ConvertToLocalFunction
            Action act = () => {
                requestPropertyAct(monster);
            };

            return act;
        }

        private void UnsupportedMonsterComponent(Action act)
        {
            act.Should().Throw<NotSupportedException>();
        }
    }
}