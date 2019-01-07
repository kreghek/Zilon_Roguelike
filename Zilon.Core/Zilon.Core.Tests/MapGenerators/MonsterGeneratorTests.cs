using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.MapGenerators;
using Zilon.Core.MapGenerators.PrimitiveStyle;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Tests.MapGenerators
{
    [TestFixture]
    public class MonsterGeneratorTests
    {
        /// <summary>
        /// Тест проверяет, что если всегда будет выпадать максимальная редкость (2),
        /// то слоты всех верхних уровней редкости будут заполнены (10 за редких, 1 чемпион).
        /// </summary>
        [Test]
        public void CreateMonsters_AlwaysMaxRarityRolls_MaxHighRarityMonsters()
        {
            var schemeDict = new Dictionary<string, IMonsterScheme>
            {
                { "regular", CreateMonsterScheme("regular") },
                { "rare", CreateMonsterScheme("rare") },
                { "champion", CreateMonsterScheme("champion") }
            };

            var schemeServiceMock = new Mock<ISchemeService>();
            schemeServiceMock.Setup(x => x.GetScheme<IMonsterScheme>(It.IsAny<string>()))
                .Returns<string>(sid => schemeDict[sid]);
            var schemeService = schemeServiceMock.Object;

            var dice = new Dice(3121);
            var randomSourceMock = new Mock<MonsterGeneratorRandomSource>(dice).As<IMonsterGeneratorRandomSource>();
            randomSourceMock.CallBase = true;
            randomSourceMock.Setup(x => x.RollRarity()).Returns(2);
            randomSourceMock.Setup(x => x.RollCount()).Returns(20);
            var randomSource = randomSourceMock.Object;

            var actorList = new List<IActor>();
            var actorManagerMock = new Mock<IActorManager>();
            actorManagerMock.Setup(x => x.Add(It.IsAny<IActor>())).Callback<IActor>(a => actorList.Add(a));
            actorManagerMock.SetupGet(x => x.Items).Returns(actorList);
            var actorManager = actorManagerMock.Object;

            var survivalRandomSourceMock = new Mock<ISurvivalRandomSource>();
            var survivalRandomSource = survivalRandomSourceMock.Object;

            var monsterGenerator = new MonsterGenerator(schemeService,
                randomSource,
                actorManager,
                survivalRandomSource);


            var map = SquareMapFactory.Create(20);
            var sectorMock = new Mock<ISector>();
            var patrolRoutes = new Dictionary<IActor, IPatrolRoute>();
            sectorMock.SetupGet(x => x.PatrolRoutes).Returns(patrolRoutes);
            var sector = sectorMock.Object;

            var monsterRegions = new List<MapRegion> {
                new MapRegion(1, map.Nodes.ToArray())
            };

            var options = new MonsterGeneratorOptions
            {
                BotPlayer = new Mock<IBotPlayer>().Object,
                RegularMonsterSids = new[] { "regular" },
                RareMonsterSids = new[] { "rare" },
                ChampionMonsterSids = new[] { "champion" }
            };

            // ACT
            monsterGenerator.CreateMonsters(sector, monsterRegions, options);



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
                    Efficient = new Core.Common.Roll(6, 1)
                }
            };
            return scheme;
        }
    }
}