﻿using System.Linq;

using LightInject;

using Moq;

using NUnit.Framework;

using Zilon.Core.Client;
using Zilon.Core.Common;
using Zilon.Core.Components;
using Zilon.Core.MapGenerators.PrimitiveStyle;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Tests.Commands
{
    [TestFixture]
    public abstract class CommandTestBase
    {
        protected ServiceContainer Container;

        [SetUp]
        public async System.Threading.Tasks.Task SetUpAsync()
        {
            Container = new ServiceContainer();

            var testMap = await SquareMapFactory.CreateAsync(10);

            var sectorMock = new Mock<ISector>();
            sectorMock.SetupGet(x => x.Map).Returns(testMap);
            var sector = sectorMock.Object;

            var sectorManagerMock = new Mock<ISectorManager>();
            sectorManagerMock.SetupGet(x => x.CurrentSector).Returns(sector);
            var sectorManager = sectorManagerMock.Object;

            var actMock = new Mock<ITacticalAct>();
            var actStatScheme = new TestTacticalActStatsSubScheme
            {
                Range = new Range<int>(1, 2)
            };
            actMock.SetupGet(x => x.Stats).Returns(actStatScheme);
            var act = actMock.Object;

            var actCarrierMock = new Mock<ITacticalActCarrier>();
            actCarrierMock.SetupGet(x => x.Acts)
                .Returns(new[] { act });
            var actCarrier = actCarrierMock.Object;

            var equipmentCarrierMock = new Mock<IEquipmentCarrier>();
            equipmentCarrierMock.SetupGet(x => x.Slots).Returns(new[] { new PersonSlotSubScheme {
                Types = EquipmentSlotTypes.Hand
            } });
            var equipmentCarrier = equipmentCarrierMock.Object;

            var personMock = new Mock<IPerson>();
            personMock.SetupGet(x => x.TacticalActCarrier).Returns(actCarrier);
            personMock.SetupGet(x => x.EquipmentCarrier).Returns(equipmentCarrier);
            var person = personMock.Object;

            var actorMock = new Mock<IActor>();
            var actorNode = testMap.Nodes.OfType<HexNode>().SelectBy(0, 0);
            actorMock.SetupGet(x => x.Node).Returns(actorNode);
            actorMock.SetupGet(x => x.Person).Returns(person);
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

            var gameLoopMock = new Mock<IGameLoop>();
            var gameLoop = gameLoopMock.Object;

            var usageServiceMock = new Mock<ITacticalActUsageService>();
            var usageService = usageServiceMock.Object;

            Container.Register(factory => sectorManager, new PerContainerLifetime());
            Container.Register(factory => humanTaskSourceMock, new PerContainerLifetime());
            Container.Register(factory => playerState, new PerContainerLifetime());
            Container.Register(factory => gameLoop, new PerContainerLifetime());
            Container.Register(factory => usageService, new PerContainerLifetime());

            RegisterSpecificServices(testMap, playerStateMock);
        }

        protected abstract void RegisterSpecificServices(IMap testMap, Mock<IPlayerState> playerStateMock);
    }
}
