using System.Collections.Generic;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tests.Tactics
{
    [TestFixture]
    public class SectorTests
    {
        private Mock<ISurvivalData> _survivalDataMock;

        /// <summary>
        /// Тест проверяет, что при обновлении состояния сектора у актёра игрока в сектора падают
        /// значения характеристик выживания.
        /// </summary>
        [Test]
        public void Update_PlayerActorWithSurvival_SurvivalStatsDecremented()
        {
            // ARRANGE
            var mapMock = new Mock<IMap>();
            var map = mapMock.Object;

            var innerActorList = new List<IActor>();
            var actorManagerMock = new Mock<IActorManager>();
            actorManagerMock.SetupGet(x => x.Items).Returns(innerActorList);
            var actorManager = actorManagerMock.Object;

            var propContainerManagerMock = new Mock<IPropContainerManager>();
            var propContainerManager = propContainerManagerMock.Object;

            var traderManagerMock = new Mock<ITraderManager>();
            var traderManager = traderManagerMock.Object;

            var dropResolverMock = new Mock<IDropResolver>();
            var dropResolver = dropResolverMock.Object;

            var schemeServiceMock = new Mock<ISchemeService>();
            var schemeService = schemeServiceMock.Object;

            var sector = new Sector(map,
                actorManager,
                propContainerManager,
                traderManager,
                dropResolver,
                schemeService);

            var actorMock = CreateActorMock();
            innerActorList.Add(actorMock.Object);



            // ACT
            sector.Update();



            // ASSERT
            _survivalDataMock.Verify(x => x.Update(), Times.Once);
        }

        /// <summary>
        /// Тест проверяет, что если для сектора не заданы узлы выхода, то событие выхода не срабатывает.
        /// </summary>
        [Test]
        public void Update_NoExits_EventNotRaised()
        {
            // ARRANGE
            var mapMock = new Mock<IMap>();
            var map = mapMock.Object;

            var innerActorList = new List<IActor>();
            var actorManagerMock = new Mock<IActorManager>();
            actorManagerMock.SetupGet(x => x.Items).Returns(innerActorList);
            var actorManager = actorManagerMock.Object;

            var propContainerManagerMock = new Mock<IPropContainerManager>();
            var propContainerManager = propContainerManagerMock.Object;

            var traderManagerMock = new Mock<ITraderManager>();
            var traderManager = traderManagerMock.Object;

            var dropResolverMock = new Mock<IDropResolver>();
            var dropResolver = dropResolverMock.Object;

            var schemeServiceMock = new Mock<ISchemeService>();
            var schemeService = schemeServiceMock.Object;

            var sector = new Sector(map,
                actorManager,
                propContainerManager,
                traderManager,
                dropResolver,
                schemeService);

            var actorMock = CreateActorMock();
            actorMock.SetupGet(x => x.Owner).Returns(new Mock<HumanPlayer>().Object);
            innerActorList.Add(actorMock.Object);



            // ACT
            using (var monitor = sector.Monitor())
            {
                sector.Update();



                // ASSERT
                monitor.Should().NotRaise(nameof(sector.HumanGroupExit));
            }
        }

        private Mock<IActor> CreateActorMock()
        {
            var actorMock = new Mock<IActor>();

            var personMock = new Mock<IPerson>();
            var person = personMock.Object;
            actorMock.SetupGet(x => x.Person).Returns(person);

            _survivalDataMock = new Mock<ISurvivalData>();
            var survivalStats = new[] {
                new SurvivalStat(0,-10,10){
                    Type = SurvivalStatType.Satiety,
                    Rate = 1,
                    KeyPoints = new SurvivalStatKeyPoint[0]
                }
            };
            _survivalDataMock.Setup(x => x.Stats).Returns(survivalStats);
            var survivalData = _survivalDataMock.Object;
            personMock.SetupGet(x => x.Survival).Returns(survivalData);

            var effectCollection = new EffectCollection();
            personMock.SetupGet(x => x.Effects).Returns(effectCollection);



            return actorMock;
        }
    }
}