using System;

using JetBrains.Annotations;

using Moq;

using NUnit.Framework;

using Zilon.Core.Common;
using Zilon.Core.Components;
using Zilon.Core.Persons;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Tests.Tactics
{
    [TestFixture()]
    public class ActorActUsageHandlerTests
    {
        /// <summary>
        /// Тест проверяет, что сервис использования действий если монстр стал мёртв,
        /// то засчитывается прогресс по перкам.
        /// </summary>
        [Test]
        public void ProcessActUsage_MonsterHitByActAndKill_SetPerkProgress()
        {
            // ARRANGE

            var perkResolverMock = new Mock<IPerkResolver>();
            var perkResolver = perkResolverMock.Object;

            var actUsageRandomSourceMock = new Mock<ITacticalActUsageRandomSource>();
            actUsageRandomSourceMock.Setup(x => x.RollToHit(It.IsAny<Roll>())).Returns(6);
            actUsageRandomSourceMock.Setup(x => x.RollEfficient(It.IsAny<Roll>())).Returns(1);
            var actUsageRandomSource = actUsageRandomSourceMock.Object;

            var actUsageService = new ActorActUsageHandler(perkResolver, actUsageRandomSource);

            var personMock = new Mock<IPerson>();
            var person = personMock.Object;

            var actorMock = new Mock<IActor>();
            actorMock.SetupGet(x => x.Node).Returns(new HexNode(0, 0));
            actorMock.SetupGet(x => x.Person).Returns(person);
            var actor = actorMock.Object;

            var monsterMock = CreateOnHitMonsterMock();
            var monster = monsterMock.Object;

            var actScheme = new TestTacticalActStatsSubScheme
            {
                Offence = new TestTacticalActOffenceSubScheme
                {
                    Type = OffenseType.Tactical,
                    Impact = ImpactType.Kinetic,
                    ApRank = 10
                }
            };

            var actMock = new Mock<ITacticalAct>();
            actMock.SetupGet(x => x.Stats).Returns(actScheme);
            var act = actMock.Object;

            // ACT
            var usedActs = new TacticalActRoll(act, 1);
            actUsageService.ProcessActUsage(actor, monster, usedActs);

            // ASSERT
            perkResolverMock.Verify(x => x.ApplyProgress(
                It.Is<IJobProgress>(progress => CheckDefeateProgress(progress, monster)),
                It.IsAny<IEvolutionData>()
                ), Times.Once);
        }

        /// <summary>
        /// Тест проверяет, что действие с определённым типом наступления
        /// успешно пробивает различные типы обороны.
        /// </summary>
        [Test]
        public void ProcessActUsage_OffenceTypeVsDefenceType_Success()
        {
            // ARRANGE
            var offenceType = OffenseType.Tactical;
            var defenceType = DefenceType.TacticalDefence;
            var defenceLevel = PersonRuleLevel.Normal;
            var fakeToHitDiceRoll = 5; // 5+ - успех при нормальном уровне обороны

            var actUsageRandomSourceMock = new Mock<ITacticalActUsageRandomSource>();
            actUsageRandomSourceMock.Setup(x => x.RollToHit(It.IsAny<Roll>())).Returns(fakeToHitDiceRoll);
            actUsageRandomSourceMock.Setup(x => x.RollEfficient(It.IsAny<Roll>())).Returns(1);
            var actUsageRandomSource = actUsageRandomSourceMock.Object;

            var perkResolverMock = new Mock<IPerkResolver>();
            var perkResolver = perkResolverMock.Object;

            var actUsageService = new ActorActUsageHandler(perkResolver, actUsageRandomSource);

            var actorMock = new Mock<IActor>();
            actorMock.SetupGet(x => x.Node).Returns(new HexNode(0, 0));
            var actor = actorMock.Object;

            var defences = new[] { new PersonDefenceItem(defenceType, defenceLevel) };
            var monsterMock = CreateMonsterMock(defences);
            var monster = monsterMock.Object;

            // Настройка дествия
            var actScheme = new TestTacticalActStatsSubScheme
            {
                Offence = new TestTacticalActOffenceSubScheme
                {
                    Type = offenceType
                }
            };

            var actMock = new Mock<ITacticalAct>();
            actMock.SetupGet(x => x.Stats).Returns(actScheme);
            var act = actMock.Object;

            // ACT
            var usedActs = new TacticalActRoll(act, 1);
            actUsageService.ProcessActUsage(actor, monster, usedActs);

            // ASSERT
            monsterMock.Verify(x => x.TakeDamage(It.IsAny<int>()), Times.Once);
        }

        private static Mock<IActor> CreateOnHitMonsterMock([CanBeNull] PersonDefenceItem[] defences = null,
            [CanBeNull] PersonArmorItem[] armors = null)
        {
            var monsterMock = new Mock<IActor>();
            monsterMock.SetupGet(x => x.Node).Returns(new HexNode(1, 0));

            var monsterPersonMock = new Mock<IPerson>();

            var monsterIsDead = false;
            var monsterSurvivalDataMock = new Mock<ISurvivalData>();
            monsterSurvivalDataMock.SetupGet(x => x.IsDead).Returns(() => monsterIsDead);
            monsterSurvivalDataMock
                .Setup(x => x.DecreaseStat(
                    It.Is<SurvivalStatType>(s => s == SurvivalStatType.Health),
                    It.IsAny<int>())
                    )
                .Callback(() => monsterIsDead = true);
            var monsterSurvival = monsterSurvivalDataMock.Object;
            monsterPersonMock.SetupGet(x => x.Survival).Returns(monsterSurvival);

            var monsterCombatStatsMock = new Mock<ICombatStats>();
            var monsterCombatStats = monsterCombatStatsMock.Object;
            monsterPersonMock.SetupGet(x => x.CombatStats).Returns(monsterCombatStats);

            var monsterPerson = monsterPersonMock.Object;
            monsterMock.SetupGet(x => x.Person).Returns(monsterPerson);

            var monsterDefenceStatsMock = new Mock<IPersonDefenceStats>();
            monsterDefenceStatsMock.SetupGet(x => x.Defences).Returns(defences ?? Array.Empty<PersonDefenceItem>());
            monsterDefenceStatsMock.SetupGet(x => x.Armors).Returns(armors ?? Array.Empty<PersonArmorItem>());
            var monsterDefenceStats = monsterDefenceStatsMock.Object;
            monsterCombatStatsMock.SetupGet(x => x.DefenceStats).Returns(monsterDefenceStats);

            monsterMock.Setup(x => x.TakeDamage(It.IsAny<int>()))
                .Callback<int>(damage => monsterSurvival.DecreaseStat(SurvivalStatType.Health, damage))
                .Verifiable();

            return monsterMock;
        }

        private static Mock<IActor> CreateMonsterMock([CanBeNull] PersonDefenceItem[] defences = null,
            [CanBeNull] PersonArmorItem[] armors = null)
        {
            var monsterMock = new Mock<IActor>();
            monsterMock.SetupGet(x => x.Node).Returns(new HexNode(1, 0));

            var monsterPersonMock = new Mock<IPerson>();

            var monsterSurvivalDataMock = new Mock<ISurvivalData>();
            monsterSurvivalDataMock.SetupGet(x => x.IsDead).Returns(false);
            var monsterSurvival = monsterSurvivalDataMock.Object;
            monsterPersonMock.SetupGet(x => x.Survival).Returns(monsterSurvival);

            var monsterCombatStatsMock = new Mock<ICombatStats>();
            var monsterCombatStats = monsterCombatStatsMock.Object;
            monsterPersonMock.SetupGet(x => x.CombatStats).Returns(monsterCombatStats);

            var monsterPerson = monsterPersonMock.Object;
            monsterMock.SetupGet(x => x.Person).Returns(monsterPerson);

            var monsterDefenceStatsMock = new Mock<IPersonDefenceStats>();
            monsterDefenceStatsMock.SetupGet(x => x.Defences).Returns(defences ?? Array.Empty<PersonDefenceItem>());
            monsterDefenceStatsMock.SetupGet(x => x.Armors).Returns(armors ?? Array.Empty<PersonArmorItem>());
            var monsterDefenceStats = monsterDefenceStatsMock.Object;
            monsterCombatStatsMock.SetupGet(x => x.DefenceStats).Returns(monsterDefenceStats);

            monsterMock.Setup(x => x.TakeDamage(It.IsAny<int>())).Verifiable();

            return monsterMock;
        }

        private static bool CheckDefeateProgress(IJobProgress progress, IAttackTarget expectedTarget)
        {
            if (progress is DefeatActorJobProgress defeatProgress)
            {
                return defeatProgress.Target == expectedTarget;
            }

            return false;
        }
    }
}