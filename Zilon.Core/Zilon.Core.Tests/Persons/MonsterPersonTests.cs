using System;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Tests.Persons
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
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

            var survivalRandomSourceMock = new Mock<ISurvivalRandomSource>();
            var survivalRandomSource = survivalRandomSourceMock.Object;

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
        /// Тест проверяет, что для у монстров нет инвентаря.
        /// Сейчас монстры в принципе генерят лут после смерти на основе своих таблиц дропа.
        /// Этот тест может устареть, когда появятся монстры-персонажи с инвентарём. Например, всякие бандиты.
        /// Сейчас же монстры достаточно одноклеточные.
        /// </summary>
        [Test]
        public void Inventory_ShouldBeNull()
        {
            // ARRANGE
            var monster = CreateMonster();

            //ACT
            var module = monster.GetModuleSafe<IInventoryModule>();

            // ASSERT
            module.Should().BeNull();
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
            Action act = () =>
            {
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