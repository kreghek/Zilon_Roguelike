using FluentAssertions;
using NUnit.Framework;
using Zilon.Logic.Tactics.Initialization;
using Zilon.Logic.Tests.Services;

namespace Zilon.Logic.Services.Tests
{
    [TestFixture()]
    public class CombatServiceTests
    {
        /// <summary>
        /// 1. В системе нужно инициировать вый бой.
        /// 2. Создаём бой.
        /// 3. Бой успешно создан.
        /// </summary>
        [Test] 
        [TestCaseSource(typeof(CombatServiceTestCaseGenerator),
            nameof(CombatServiceTestCaseGenerator.CreateCombatCorrectTestCases))]
        public void CreateCombat_Default_CombatCreated(CombatInitData initData)
        {
            // ARRANGE
            var combatService = CreateCombatService();

            // ACT
            var combat = combatService.CreateCombat(initData);


            // ASSERT
            combat.Should().NotBeNull();
        }

        private CombatService CreateCombatService()
        {
            var combatService = new CombatService();
            return combatService;
        }
    }
}