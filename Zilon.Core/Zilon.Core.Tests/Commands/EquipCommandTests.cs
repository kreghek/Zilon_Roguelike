using System.Linq;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Client;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.TestCommon;

namespace Zilon.Core.Commands.Tests
{
    [TestFixture]
    public class EquipCommandTests
    {
        /// <summary>
        /// Тест проверяет, что можно использовать экипировку.
        /// </summary>
        [Test]
        public void CanExecuteTest()
        {
            // ARRANGE
            var actorManagerMock = new Mock<IActorManager>();
            var actorManager = actorManagerMock.Object;

            var containerManagerMock = new Mock<IPropContainerManager>();
            var containerManager = containerManagerMock.Object;

            var testMap = new TestGridGenMap();
            var sector = new Sector(testMap, actorManager, containerManager);

            var sectorManagerMock = new Mock<ISectorManager>();
            sectorManagerMock.SetupProperty(x => x.CurrentSector, sector);
            var sectorManager = sectorManagerMock.Object;


            var actorMock = new Mock<IActor>();
            var actorNode = testMap.Nodes.OfType<HexNode>().SelectBy(0, 0);
            actorMock.SetupGet(x=>x.Node).Returns(actorNode);
            var actor = actorMock.Object;

            var actorVmMock = new Mock<IActorViewModel>();
            actorVmMock.SetupProperty(x => x.Actor, actor);
            var actorVm = actorVmMock.Object;

            var targetMock = new Mock<IActor>();
            var targetNode = testMap.Nodes.OfType<HexNode>().SelectBy(2, 0);
            targetMock.SetupGet(x => x.Node).Returns(targetNode);
            var target = targetMock.Object;

            var targetVmMock = new Mock<IActorViewModel>();
            targetVmMock.SetupProperty(x => x.Actor, target);
            var targetVm = targetVmMock.Object;

            var playerStateMock = new Mock<IPlayerState>();
            playerStateMock.SetupProperty(x => x.ActiveActor, actorVm);
            playerStateMock.SetupProperty(x => x.HoverViewModel, targetVm);
            var playerState = playerStateMock.Object;


            var propScheme = new PropScheme
            {
                Equip = new PropEquipSubScheme
                {

                }
            };
            var equipment = new Equipment(propScheme, new TacticalActScheme[0]);

            var equipmentViewModelMock = new Mock<IPropItemViewModel>();
            equipmentViewModelMock.SetupGet(x => x.Prop).Returns(equipment);
            var equipmentViewModel = equipmentViewModelMock.Object;

            var inventoryStateMock = new Mock<IInventoryState>();
            inventoryStateMock.SetupProperty(x => x.SelectedProp, equipmentViewModel);
            var inventoryState = inventoryStateMock.Object;

            var command = new EquipCommand(sectorManager, playerState, inventoryState);
            command.SlotIndex = 0;



            // ACT
            var canExecute = command.CanExecute();


            // ASSERT
            canExecute.Should().Be(true);
        }

        /// <summary>
        /// Тест проверяет, что при выполнении команды корректно фисируется намерение игрока на атаку.
        /// </summary>
        [Test]
        [Ignore("Привести к EquipCommand")]
        public void ExecuteTest()
        {
            var testMap = new TestGridGenMap();

            var sectorMock = new Mock<ISector>();
            sectorMock.SetupGet(x => x.Map).Returns(testMap);
            var sector = sectorMock.Object;

            var sectorManagerMock = new Mock<ISectorManager>();
            sectorManagerMock.SetupProperty(x => x.CurrentSector, sector);
            var sectorManager = sectorManagerMock.Object;


            var actorMock = new Mock<IActor>();
            var actorNode = testMap.Nodes.OfType<HexNode>().SelectBy(0, 0);
            actorMock.SetupGet(x => x.Node).Returns(actorNode);
            var actor = actorMock.Object;

            var actorVmMock = new Mock<IActorViewModel>();
            actorVmMock.SetupProperty(x => x.Actor, actor);
            var actorVm = actorVmMock.Object;

            var targetMock = new Mock<IActor>();
            var targetNode = testMap.Nodes.OfType<HexNode>().SelectBy(2, 0);
            targetMock.SetupGet(x => x.Node).Returns(targetNode);
            var target = targetMock.Object;

            var targetVmMock = new Mock<IActorViewModel>();
            targetVmMock.SetupProperty(x => x.Actor, target);
            var targetVm = targetVmMock.Object;

            var decisionSourceMock = new Mock<IDecisionSource>();
            var decisionSource = decisionSourceMock.Object;

            var humanTaskSourceMock = new Mock<HumanActorTaskSource>(actor, decisionSource);
            var humanTaskSource = humanTaskSourceMock.Object;

            var playerStateMock = new Mock<IPlayerState>();
            playerStateMock.SetupProperty(x => x.ActiveActor, actorVm);
            playerStateMock.SetupProperty(x => x.HoverViewModel, targetVm);
            playerStateMock.SetupProperty(x => x.TaskSource, humanTaskSource);
            var playerState = playerStateMock.Object;

            var command = new AttackCommand(sectorManager, playerState);



            // ACT
            command.Execute();


            // ASSERT
            humanTaskSourceMock.Verify(x => x.IntentAttack(target));
        }
    }
}