using System.Collections.Generic;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Persons.Survival;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tests.Tactics
{
    [TestFixture]
    public class SectorTests
    {
        private Mock<ISurvivalModule> _survivalDataMock;

        /// <summary>
        /// Тест проверяет, что если для сектора не заданы узлы выхода, то событие выхода не срабатывает.
        /// </summary>
        [Test]
        public void Update_NoExits_EventNotRaised()
        {
            // ARRANGE
            var mapMock = new Mock<ISectorMap>();
            var map = mapMock.Object;

            var innerActorList = new List<IActor>();
            var actorManagerMock = new Mock<IActorManager>();
            actorManagerMock.SetupGet(x => x.Items).Returns(innerActorList);
            var actorManager = actorManagerMock.Object;

            var propContainerManagerMock = new Mock<IStaticObjectManager>();
            var propContainerManager = propContainerManagerMock.Object;

            var dropResolverMock = new Mock<IDropResolver>();
            var dropResolver = dropResolverMock.Object;

            var schemeServiceMock = new Mock<ISchemeService>();
            var schemeService = schemeServiceMock.Object;

            var equipmentDurableServiceMock = new Mock<IEquipmentDurableService>();
            var equipmentDurableService = equipmentDurableServiceMock.Object;

            var sector = new Sector(map,
                actorManager,
                propContainerManager,
                dropResolver,
                schemeService,
                equipmentDurableService);

            var actorMock = CreateActorMock();
            innerActorList.Add(actorMock.Object);

            // ACT
            using var monitor = sector.Monitor();
            sector.Update();

            // ASSERT
            monitor.Should().NotRaise(nameof(sector.TrasitionUsed));
        }

        /// <summary>
        /// Тест проверяет, что при обновлении состояния сектора у актёра игрока в сектора падают
        /// значения характеристик выживания.
        /// </summary>
        [Test]
        public void Update_PlayerActorWithSurvival_SurvivalStatsDecremented()
        {
            // ARRANGE
            var mapMock = new Mock<ISectorMap>();
            var map = mapMock.Object;

            var innerActorList = new List<IActor>();
            var actorManagerMock = new Mock<IActorManager>();
            actorManagerMock.SetupGet(x => x.Items).Returns(innerActorList);
            var actorManager = actorManagerMock.Object;

            var propContainerManagerMock = new Mock<IStaticObjectManager>();
            var propContainerManager = propContainerManagerMock.Object;

            var dropResolverMock = new Mock<IDropResolver>();
            var dropResolver = dropResolverMock.Object;

            var schemeServiceMock = new Mock<ISchemeService>();
            var schemeService = schemeServiceMock.Object;

            var equipmentDurableServiceMock = new Mock<IEquipmentDurableService>();
            var equipmentDurableService = equipmentDurableServiceMock.Object;

            var sector = new Sector(map,
                actorManager,
                propContainerManager,
                dropResolver,
                schemeService,
                equipmentDurableService);

            var actorMock = CreateActorMock();
            innerActorList.Add(actorMock.Object);

            // ACT
            sector.Update();

            // ASSERT
            _survivalDataMock.Verify(x => x.Update(), Times.Once);
        }

        private Mock<IActor> CreateActorMock()
        {
            var actorMock = new Mock<IActor>();

            var personMock = new Mock<IPerson>();
            var person = personMock.Object;
            actorMock.SetupGet(x => x.Person).Returns(person);

            _survivalDataMock = new Mock<ISurvivalModule>();
            var survivalStats = new[]
            {
                new SurvivalStat(0, -10, 10)
                {
                    Type = SurvivalStatType.Satiety,
                    Rate = 1
                }
            };
            _survivalDataMock.Setup(x => x.Stats).Returns(survivalStats);
            var survivalData = _survivalDataMock.Object;
            personMock.Setup(x => x.GetModule<ISurvivalModule>(It.IsAny<string>())).Returns(survivalData);
            personMock.Setup(x => x.HasModule(It.Is<string>(x => x == nameof(ISurvivalModule)))).Returns(true);

            var effectCollection = new EffectsModule();
            personMock.Setup(x => x.GetModule<IEffectsModule>(It.IsAny<string>())).Returns(effectCollection);

            return actorMock;
        }
    }
}