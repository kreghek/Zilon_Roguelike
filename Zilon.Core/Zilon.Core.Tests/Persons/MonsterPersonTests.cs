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

        /// <summary>
        /// Тест проверяет, что для монстров выбрасывается сообщение на неподдерживаемые компоненты (Развитие).
        /// </summary>
        [Test]
        public void EvolutionData_ThrowNotSupported()
        {
            // ARRANGE
            var monster = CreateMonster();

            Action<MonsterPerson> requestPropertyAct = m =>
            {
                var tmp = m.EvolutionData;
            };

            //ACT
            var act = ActUnsupportedMonsterComponent(monster, requestPropertyAct);


            // ASSERT
            UnsupportedMonsterComponent(act);
        }

        /// <summary>
        /// Тест проверяет, что для монстров выбрасывается сообщение на неподдерживаемые компоненты (Боевые характеристики).
        /// </summary>
        [Test]
        public void CombatStats_ThrowNotSupported()
        {
            // ARRANGE
            var monster = CreateMonster();

            Action<MonsterPerson> requestPropertyAct = m =>
            {
                var tmp = m.CombatStats;
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

            Action<MonsterPerson> requestPropertyAct = m =>
            {
                var tmp = m.Inventory;
            };

            //ACT
            var act = ActUnsupportedMonsterComponent(monster, requestPropertyAct);


            // ASSERT
            UnsupportedMonsterComponent(act);
        }

        /// <summary>
        /// Тест проверяет, что для монстров данные о выживании равны null.
        /// Это нужно, когда сектор обновляет состояния выживания. Чтобы избежать сравнение типов, там есть проверка на null.
        /// </summary>
        [Test]
        public void Survival_ReturnsNull()
        {
            // ARRANGE
            var monster = CreateMonster();

            //ACT
            var factSurvival = monster.Survival;


            // ASSERT
            factSurvival.Should().BeNull();
        }

        private static MonsterPerson CreateMonster()
        {
            var monsterScheme = new MonsterScheme
            {
                PrimaryAct = new TacticalActStatsSubScheme
                {
                    Efficient = new Range<float>(1, 1)
                }
            };
            var monster = new MonsterPerson(monsterScheme);
            return monster;
        }

        private Action ActUnsupportedMonsterComponent(MonsterPerson monster, Action<MonsterPerson> requestPropertyAct)
        {
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