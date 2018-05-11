namespace Zilon.Core.Tests.Services
{
    using System.Linq;

    using FluentAssertions;

    using Moq;

    using NUnit.Framework;

    using Zilon.Core.Persons;
    using Zilon.Core.Players;
    using Zilon.Core.Services;
    using Zilon.Core.Tactics.Events;
    using Zilon.Core.Tactics.Initialization;
    using Zilon.Core.Tactics.Map;
    using Zilon.Core.Tests.TestCommon;

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

        /// <summary>
        /// 1. В системе есть бой. Взвод должен переместиться на произвольную свободную ноду.
        /// 2. Перемещаем взвод.
        /// 3. Получаем событие, содержащее информацию об успешном перемещении.
        /// </summary>
        [Test]
        public void MoveCommand_MoveToRandomCorrectNode_HasMoveEvent()
        {
            // ARRANGE
            var combatService = CreateCombatService();
            var initData = CreateInitData();

            var combat = combatService.CreateCombat(initData);
            var squad = initData.Players.First().Squads.First();
            var actorSquad = combat.Squads.SingleOrDefault(x => x.Squad == squad);
            var targetNode = combat.Map.Nodes.FirstOrDefault(x => x != actorSquad.Node);


            // ACT
            var commandResult = combatService.MoveCommand(combat, actorSquad, targetNode);


            // ASSERT
            commandResult.Type.Should().Be(CommandResultType.Complete);
            commandResult.Errors.Should().BeNullOrEmpty();
            commandResult.Events.Should().NotBeNullOrEmpty();

            var eventGroup = commandResult.Events.First() as EventGroup;
            var moveEvent = eventGroup.Events.First() as SquadMovedEvent;
            moveEvent.FinishNodeId.Should().Be(targetNode.Id);
        }

        private static CombatInitData CreateInitData()
        {
            var mapGenerator = MapGeneratorMocks.CreateTwoNodesMapGenerator();
            var map = new CombatMap();
            mapGenerator.CreateMap(map);

            return new CombatInitData
            {
                Map = map,
                Players = new[] {
                    new PlayerCombatInitData{
                        Player = new Mock<IPlayer>().Object,
                        Squads = new[]{
                            new Squad{
                                Persons = new []{
                                    new Person()
                                }
                            }
                        }
                    }
                }
            };
        }

        private static CombatService CreateCombatService()
        {
            var combatCommandResolver = new CombatCommandResolver();
            var combatService = new CombatService(combatCommandResolver);
            return combatService;
        }

        
    }
}