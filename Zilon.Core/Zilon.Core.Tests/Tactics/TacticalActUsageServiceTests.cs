using Moq;

using NUnit.Framework;

using Zilon.Core.Common;
using Zilon.Core.Components;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common;

namespace Zilon.Core.Tests.Tactics
{
    [TestFixture]
    public class TacticalActUsageServiceTests
    {
        private ITacticalActUsageRandomSource _actUsageRandomSource;
        private Mock<IPerkResolver> _perkResolverMock;
        private IPerkResolver _perkResolver;
        private ITacticalAct _act;
        private IPerson _person;

        /// <summary>
        /// Тест проверяет, что сервис использования действий корректно наносит урон целевому монстру.
        /// </summary>
        [Test]
        [Ignore("Скорее всего устарел после появления типов наступления и защиты.")]
        public void UseOn_MonsterHitByAct_MonsterTakesDamage()
        {
            // ARRANGE
            
            var actUsageService = new TacticalActUsageService(_actUsageRandomSource, _perkResolver);

            var actorMock = new Mock<IActor>();
            actorMock.SetupGet(x => x.Node).Returns(new HexNode(0, 0));
            var actor = actorMock.Object;

            var monsterMock = new Mock<IActor>();
            monsterMock.SetupGet(x => x.Node).Returns(new HexNode(1, 0));
            var monster = monsterMock.Object;

            var monsterStateMock = new Mock<IActorState>();
            monsterStateMock.SetupGet(x => x.IsDead).Returns(false);
            var monsterState = monsterStateMock.Object;
            monsterMock.SetupGet(x => x.State).Returns(monsterState);



            // ACT
            actUsageService.UseOn(actor, monster, _act);



            // ASSERT
            monsterMock.Verify(x => x.TakeDamage(It.IsAny<float>()), Times.Once);
        }

        /// <summary>
        /// Тест проверяет, что сервис использования действий если монстр стал мёртв, то засчитывается прогресс по перкам.
        /// </summary>
        [Test]
        public void UseOn_MonsterHitByActAndKill_SetPerkProgress()
        {
            // ARRANGE

            var actUsageService = new TacticalActUsageService(_actUsageRandomSource, _perkResolver);

            var actorMock = new Mock<IActor>();
            actorMock.SetupGet(x => x.Node).Returns(new HexNode(0, 0));
            actorMock.SetupGet(x => x.Person).Returns(_person);
            var actor = actorMock.Object;

            var monsterMock = new Mock<IActor>();
            monsterMock.SetupGet(x => x.Node).Returns(new HexNode(1, 0));
            var monster = monsterMock.Object;

            var monsterStateMock = new Mock<IActorState>();
            var monsterState = monsterStateMock.Object;
            monsterMock.SetupGet(x => x.State).Returns(monsterState);

            var monsterIsDead = false;
            monsterStateMock.SetupGet(x => x.IsDead).Returns(() => monsterIsDead);
            monsterStateMock.Setup(x => x.TakeDamage(It.IsAny<float>())).Callback(() => monsterIsDead = true);
            monsterMock.Setup(x => x.TakeDamage(It.IsAny<float>())).Callback<float>(damage => monsterState.TakeDamage(damage));



            // ACT
            actUsageService.UseOn(actor, monster, _act);



            // ASSERT
            _perkResolverMock.Verify(x => x.ApplyProgress(
                It.Is<IJobProgress>(progress => CheckDefeateProgress(progress, monster)),
                It.IsAny<IEvolutionData>()
                ), Times.Once);
        }

        /// <summary>
        /// Тест проверяет, что действием с определённым типом наспления успешно выполняется при различных типах обороны.
        /// </summary>
        [Test]
        public void UseOn_OffenceTypeVsDefenceType_Success()
        {
            // ARRANGE
            var offenceType = OffenseType.Tactical;
            var defenceType = DefenceType.TacticalDefence;
            var fakeDiceRoll = 5; // 5+ - успех


            var actUsageService = new TacticalActUsageService(_actUsageRandomSource, _perkResolver);

            var actorMock = new Mock<IActor>();
            actorMock.SetupGet(x => x.Node).Returns(new HexNode(0, 0));
            var actor = actorMock.Object;

            var monsterMock = new Mock<IActor>();
            monsterMock.SetupGet(x => x.Node).Returns(new HexNode(1, 0));
            var monster = monsterMock.Object;

            var monsterStateMock = new Mock<IActorState>();
            monsterStateMock.SetupGet(x => x.IsDead).Returns(false);
            var monsterState = monsterStateMock.Object;
            monsterMock.SetupGet(x => x.State).Returns(monsterState);

            // Настройка дествия
            var actScheme = new TacticalActStatsSubScheme {
                Range = new Range<int>(1, 1),
                Offence = new TestTacticalActOffenceSubScheme {
                    Type = offenceType
                }
            };

            var actMock = new Mock<ITacticalAct>();
            actMock.SetupGet(x => x.Stats).Returns(actScheme);
            var act = actMock.Object;



            // ACT
            actUsageService.UseOn(actor, monster, act);



            // ASSERT
            monsterMock.Verify(x => x.TakeDamage(It.IsAny<float>()), Times.Once);
        }

        private static bool CheckDefeateProgress(IJobProgress progress, IAttackTarget expectedTarget)
        {
            if (progress is DefeatActorJobProgress defeatProgress)
            {
                return defeatProgress.Target == expectedTarget;
            }

            return false;
        }

        [SetUp]
        public void SetUp()
        {
            var actUsageRandomSourceMock = new Mock<ITacticalActUsageRandomSource>();
            _actUsageRandomSource = actUsageRandomSourceMock.Object;

            _perkResolverMock = new Mock<IPerkResolver>();
            _perkResolver = _perkResolverMock.Object;

            var personMock = new Mock<IPerson>();
            _person = personMock.Object;

            var evolutionDataMock = new Mock<IEvolutionData>();
            var evolutionData = evolutionDataMock.Object;
            personMock.SetupGet(x => x.EvolutionData).Returns(evolutionData);

            var actMock = new Mock<ITacticalAct>();
            actMock.SetupGet(x => x.Stats).Returns(new TacticalActStatsSubScheme { Range = new Range<int>(1, 1) });
            _act = actMock.Object;
        }
    }
}