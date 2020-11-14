using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Common;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.MapGenerators;
using Zilon.Core.MapGenerators.PrimitiveStyle;
using Zilon.Core.PersonGeneration;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Tests.MapGenerators
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class MonsterGeneratorTests
    {
        /// <summary>
        /// Тест проверяет, что если всегда будет выпадать максимальная редкость (2),
        /// то слоты всех верхних уровней редкости будут заполнены (10 за редких, 1 чемпион).
        /// </summary>
        [Test]
        public async Task CreateMonsters_AlwaysMaxRarityRolls_MaxHighRarityMonstersAsync()
        {
            var schemeDict = new Dictionary<string, IMonsterScheme>
            {
                {
                    "regular", CreateMonsterScheme("regular")
                },
                {
                    "rare", CreateMonsterScheme("rare")
                },
                {
                    "champion", CreateMonsterScheme("champion")
                }
            };

            var schemeServiceMock = new Mock<ISchemeService>();
            schemeServiceMock.Setup(x => x.GetScheme<IMonsterScheme>(It.IsAny<string>()))
                .Returns<string>(sid => schemeDict[sid]);
            var schemeService = schemeServiceMock.Object;

            var dice = new LinearDice(3121);
            var randomSourceMock = new Mock<MonsterGeneratorRandomSource>(dice).As<IMonsterGeneratorRandomSource>();
            randomSourceMock.CallBase = true;
            randomSourceMock.Setup(x => x.RollRarity()).Returns(2);
            randomSourceMock.Setup(x => x.RollRegionCount(It.IsAny<int>(), It.IsAny<int>())).Returns(20);
            var randomSource = randomSourceMock.Object;

            var actorList = new List<IActor>();
            var actorManagerMock = new Mock<IActorManager>();
            actorManagerMock.Setup(x => x.Add(It.IsAny<IActor>())).Callback<IActor>(a => actorList.Add(a));
            actorManagerMock.SetupGet(x => x.Items).Returns(actorList);
            var actorManager = actorManagerMock.Object;

            var propContainerManagerMock = new Mock<IStaticObjectManager>();
            var propContainerManager = propContainerManagerMock.Object;
            propContainerManagerMock.SetupGet(x => x.Items).Returns(Array.Empty<IStaticObject>());

            var taskSourceMock = new Mock<IActorTaskSource<ISectorTaskSourceContext>>();
            var taskSource = taskSourceMock.Object;

            var monsterFactory = new MonsterPersonFactory();

            var monsterGenerator = new MonsterGenerator(schemeService,
                monsterFactory,
                randomSource,
                taskSource);

            var map = await SquareMapFactory.CreateAsync(20).ConfigureAwait(false);

            var sectorMock = new Mock<ISector>();
            var patrolRoutes = new Dictionary<IActor, IPatrolRoute>();
            sectorMock.SetupGet(x => x.PatrolRoutes).Returns(patrolRoutes);
            sectorMock.SetupGet(x => x.ActorManager).Returns(actorManager);
            sectorMock.SetupGet(x => x.StaticObjectManager).Returns(propContainerManager);
            var sector = sectorMock.Object;

            var monsterRegions = new List<MapRegion>
            {
                new MapRegion(1, map.Nodes.ToArray())
            };

            var sectorScheme = new TestSectorSubScheme
            {
                RegularMonsterSids = new[]
                {
                    "regular"
                },
                RareMonsterSids = new[]
                {
                    "rare"
                },
                ChampionMonsterSids = new[]
                {
                    "champion"
                }
            };

            // ACT
            monsterGenerator.CreateMonsters(sector,
                monsterRegions,
                sectorScheme);

            // ASSERT
            var championCount = actorManager.Items.Count(x => ((MonsterPerson)x.Person).Scheme.Sid == "champion");
            championCount.Should().Be(1);

            var rareCount = actorManager.Items.Count(x => ((MonsterPerson)x.Person).Scheme.Sid == "rare");
            rareCount.Should().Be(10);
        }

        private IMonsterScheme CreateMonsterScheme(string sid)
        {
            var scheme = new TestMonsterScheme
            {
                Sid = sid,
                PrimaryAct = new TestTacticalActStatsSubScheme
                {
                    Efficient = new Roll(6, 1)
                }
            };
            return scheme;
        }
    }
}