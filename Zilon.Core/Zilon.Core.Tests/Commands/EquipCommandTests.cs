using FluentAssertions;

using LightInject;

using Moq;

using NUnit.Framework;

using System.Linq;

using Zilon.Core.Client;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.TestCommon;

namespace Zilon.Core.Commands.Tests
{
    [TestFixture]
    public class EquipCommandTests
    {
        private ServiceContainer _container;

        /// <summary>
        /// Тест проверяет, что можно использовать экипировку.
        /// </summary>
        [Test]
        public void CanExecuteTest()
        {
            // ARRANGE
            var command = _container.GetInstance<EquipCommand>();
            command.SlotIndex = 0;



            // ACT
            var canExecute = command.CanExecute();


            // ASSERT
            canExecute.Should().Be(true);
        }

        [SetUp]
        public void SetUp ()
        {
            _container = new ServiceContainer();

            var testMap = new TestGridGenMap(3);

            var sectorMock =  new Mock<ISector>();
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

            var humanTaskSourceMock = new Mock<IHumanActorTaskSource>();
            var humanTaskSource = humanTaskSourceMock.Object;

            var playerStateMock = new Mock<IPlayerState>();
            playerStateMock.SetupProperty(x => x.ActiveActor, actorVm);
            playerStateMock.SetupProperty(x => x.TaskSource, humanTaskSource);
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

            _container.Register(factory => sectorManager, new PerContainerLifetime());
            _container.Register(factory => humanTaskSourceMock, new PerContainerLifetime());
            _container.Register(factory => playerState, new PerContainerLifetime());
            _container.Register(factory => inventoryState, new PerContainerLifetime());
            _container.Register<EquipCommand>(new PerContainerLifetime());
        }

        /// <summary>
        /// Тест проверяет, что при выполнении команды корректно фисируется намерение игрока на атаку.
        /// </summary>
        [Test]
        public void ExecuteTest()
        {
            var command = _container.GetInstance<EquipCommand>();
            command.SlotIndex = 0;

            var humanTaskSourceMock = _container.GetInstance<Mock<IHumanActorTaskSource>>();



            // ACT
            command.Execute();


            // ASSERT
            //humanTaskSourceMock.Verify(x => x.IntentEquip(It.IsAny<Equipment>(), 0));
            humanTaskSourceMock.Verify(x => x.Intent(null));
        }
    }
}