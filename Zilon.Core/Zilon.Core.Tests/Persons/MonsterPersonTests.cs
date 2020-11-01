using System;
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
        ///     Тест проверяет, что нет исключений при создании монстра.
        /// </summary>
        [Test]
        public void Constructor_DefaultParams_NoException()
        {
            // ARRANGE
            TestMonsterScheme monsterScheme = new TestMonsterScheme {PrimaryAct = new TestTacticalActStatsSubScheme()};

            var survivalRandomSourceMock = new Mock<ISurvivalRandomSource>();
            var survivalRandomSource = survivalRandomSourceMock.Object;

            // ACT
            Action act = () =>
            {
                // ReSharper disable once UnusedVariable
                MonsterPerson monster = new MonsterPerson(monsterScheme);
            };

            // ARRANGE
            act.Should().NotThrow();
        }

        /// <summary>
        ///     Тест проверяет, что для монстров выбрасывается сообщение на не поддерживаемые компоненты (Развитие).
        /// </summary>
        [Test]
        public void EvolutionData_ThrowNotSupported()
        {
            // ARRANGE
            MonsterPerson monster = CreateMonster();

            //ACT
            IEvolutionModule module = monster.GetModuleSafe<IEvolutionModule>();

            // ASSERT
            module.Should().BeNull();
        }

        /// <summary>
        ///     Тест проверяет, что для у монстров нет инвентаря.
        ///     Сейчас монстры в принципе генерят лут после смерти на основе своих таблиц дропа.
        ///     Этот тест может устареть, когда появятся монстры-персонажи с инвентарём. Например, всякие бандиты.
        ///     Сейчас же монстры достаточно одноклеточные.
        /// </summary>
        [Test]
        public void Inventory_ShouldBeNull()
        {
            // ARRANGE
            MonsterPerson monster = CreateMonster();

            //ACT
            IInventoryModule module = monster.GetModuleSafe<IInventoryModule>();

            // ASSERT
            module.Should().BeNull();
        }

        private static MonsterPerson CreateMonster()
        {
            TestMonsterScheme monsterScheme = new TestMonsterScheme {PrimaryAct = new TestTacticalActStatsSubScheme()};

            MonsterPerson monster = new MonsterPerson(monsterScheme);
            return monster;
        }
    }
}