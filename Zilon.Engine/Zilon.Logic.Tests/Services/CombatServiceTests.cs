using Zilon.Logic.Services;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Zilon.Logic.Tactics.Initialization;
using Zilon.Logic.Tests.Services;
using Zilon.Logic.Tactics.Map;
using Zilon.Logic.Players;
using Zilon.Logic.Persons;
using System.Linq;
using Zilon.Logic.Tactics.Events;

namespace Zilon.Logic.Services.Tests
{
    [TestFixture]
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

        [Test]
        public void MoveCommandTest()
        {
            // ARRANGE
            var combatService = CreateCombatService();

            var initData = new CombatInitData {
                Map = new CombatMap(),
                Players = new [] {
                    new PlayerCombatInitData{
                        Player = new Mock<IPlayer>().Object,
                        Squads = new Squad[]{
                            new Squad{
                                Persons = new []{
                                    new Person{
                                        
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var combat = combatService.CreateCombat(initData);
            var squad = initData.Players.First().Squads.First();
            var actorSquad = combat.Squads.SingleOrDefault(x=>x.Squad == squad);
            var targetNode = combat.Map.Nodes.FirstOrDefault(x=>x != actorSquad.Node);


            // ACT
            var commandResult = combatService.MoveCommand(combat, actorSquad, targetNode);


            // ASSERT
            commandResult.Type.Should().Be(CommandResultType.Complete);
            commandResult.Errors.Should().BeNullOrEmpty();
            commandResult.Events.Should().NotBeNullOrEmpty();
        }

        private CombatService CreateCombatService()
        {
            
            var combatCommandResolver = new CombatCommandResolver();
            var combatService = new CombatService(combatCommandResolver);
            return combatService;
        }

        
    }
}