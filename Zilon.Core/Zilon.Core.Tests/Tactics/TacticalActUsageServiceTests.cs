﻿using JetBrains.Annotations;
using Moq;

using NUnit.Framework;

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
        /// Тест проверяет, что сервис использования действий если монстр стал мёртв,
        /// то засчитывается прогресс по перкам.
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

            var monsterMock = CreateMonsterMock();
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
        /// Тест проверяет, что действием с определённым типом наступления
        /// успешно выполняется при различных типах обороны.
        /// </summary>
        [Test]
        public void UseOn_OffenceTypeVsDefenceType_Success()
        {
            // ARRANGE
            var offenceType = OffenseType.Tactical;
            var defenceType = DefenceType.TacticalDefence;
            var defenceLevel = PersonRuleLevel.Normal;
            var fakeToHitDiceRoll = 5; // 5+ - успех при нормальном уровне обороны

            var actUsageRandomSourceMock = new Mock<ITacticalActUsageRandomSource>();
            actUsageRandomSourceMock.Setup(x => x.RollToHit()).Returns(fakeToHitDiceRoll);
            var actUsageRandomSource = actUsageRandomSourceMock.Object;

            var actUsageService = new TacticalActUsageService(actUsageRandomSource, _perkResolver);

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
            actUsageService.UseOn(actor, monster, act);



            // ASSERT
            monsterMock.Verify(x => x.TakeDamage(It.IsAny<float>()), Times.Once);
        }

        /// <summary>
        /// Тест проверяет, что если действие имеет больший ранг пробития,
        /// то броня игнорируется.
        /// </summary>
        [Test]
        public void UseOn_ActApGreaterRankThatArmorRank_IgnoreArmor()
        {
            // ARRANGE
            var offenceType = OffenseType.Tactical;
            var fakeToHitDiceRoll = 2; // успех в ToHit 2+

            var actUsageRandomSourceMock = new Mock<ITacticalActUsageRandomSource>();
            actUsageRandomSourceMock.Setup(x => x.RollToHit()).Returns(fakeToHitDiceRoll);
            var actUsageRandomSource = actUsageRandomSourceMock.Object;

            var actUsageService = new TacticalActUsageService(actUsageRandomSource, _perkResolver);

            var actorMock = new Mock<IActor>();
            actorMock.SetupGet(x => x.Node).Returns(new HexNode(0, 0));
            var actor = actorMock.Object;

            var armors = new[] { new PersonArmorItem(ImpactType.Kinetic, PersonRuleLevel.Normal, 9) };
            var monsterMock = CreateMonsterMock(armors: armors);
            var monster = monsterMock.Object;

            // Настройка дествия
            var actScheme = new TestTacticalActStatsSubScheme
            {
                Offence = new TestTacticalActOffenceSubScheme
                {
                    Type = offenceType,
                    ApRank = 10,
                    Impact = ImpactType.Kinetic
                }
            };

            var actMock = new Mock<ITacticalAct>();
            actMock.SetupGet(x => x.Stats).Returns(actScheme);
            var act = actMock.Object;



            // ACT
            actUsageService.UseOn(actor, monster, act);



            // ASSERT
            monsterMock.Verify(x => x.TakeDamage(It.IsAny<float>()), Times.Once);
            actUsageRandomSourceMock.Verify(x => x.RollArmorSave(), Times.Never);
        }

        /// <summary>
        /// Тест проверяет, что броня поглощает урон.
        /// </summary>
        [Test]
        public void UseOn_ArmorSavePassed_ActEfficientDecrease()
        {
            // ARRANGE
            var offenceType = OffenseType.Tactical;
            var fakeToHitDiceRoll = 2; // успех в ToHit 2+
            var fakeToHitDiceRoll = 6; // успех в ToHit 2+

            var actUsageRandomSourceMock = new Mock<ITacticalActUsageRandomSource>();
            actUsageRandomSourceMock.Setup(x => x.RollToHit()).Returns(fakeToHitDiceRoll);
            actUsageRandomSourceMock.Setup(x => x.RollArmorSave()).Returns(fakeToHitDiceRoll);
            var actUsageRandomSource = actUsageRandomSourceMock.Object;

            var actUsageService = new TacticalActUsageService(actUsageRandomSource, _perkResolver);

            var actorMock = new Mock<IActor>();
            actorMock.SetupGet(x => x.Node).Returns(new HexNode(0, 0));
            var actor = actorMock.Object;

            var armors = new[] { new PersonArmorItem(ImpactType.Kinetic, PersonRuleLevel.Lesser, 9) };
            var monsterMock = CreateMonsterMock(armors: armors);
            var monster = monsterMock.Object;

            // Настройка дествия
            var actScheme = new TestTacticalActStatsSubScheme
            {
                Offence = new TestTacticalActOffenceSubScheme
                {
                    Type = offenceType,
                    ApRank = 10,
                    Impact = ImpactType.Kinetic
                }
            };

            var actMock = new Mock<ITacticalAct>();
            actMock.SetupGet(x => x.Stats).Returns(actScheme);
            var act = actMock.Object;



            // ACT
            actUsageService.UseOn(actor, monster, act);



            // ASSERT
            monsterMock.Verify(x => x.TakeDamage(It.IsAny<float>()), Times.Once);
            actUsageRandomSourceMock.Verify(x => x.RollArmorSave(), Times.Never);
        }

        private static Mock<IActor> CreateMonsterMock([CanBeNull] PersonDefenceItem[] defences = null,
            [CanBeNull] PersonArmorItem[] armors = null)
        {
            var monsterMock = new Mock<IActor>();
            monsterMock.SetupGet(x => x.Node).Returns(new HexNode(1, 0));

            var monsterStateMock = new Mock<IActorState>();
            monsterStateMock.SetupGet(x => x.IsDead).Returns(false);
            var monsterState = monsterStateMock.Object;
            monsterMock.SetupGet(x => x.State).Returns(monsterState);

            var monsterPersonMock = new Mock<IPerson>();
            var monsterPerson = monsterPersonMock.Object;
            monsterMock.SetupGet(x => x.Person).Returns(monsterPerson);

            var monsterCombatStatsMock = new Mock<ICombatStats>();
            var monsterCombatStats = monsterCombatStatsMock.Object;
            monsterPersonMock.SetupGet(x => x.CombatStats).Returns(monsterCombatStats);

            var monsterDefenceStatsMock = new Mock<IPersonDefenceStats>();
            monsterDefenceStatsMock.SetupGet(x => x.Defences).Returns(defences ?? new PersonDefenceItem[0]);
            monsterDefenceStatsMock.SetupGet(x => x.Armors).Returns(armors ?? new PersonArmorItem[0]);
            var monsterDefenceStats = monsterDefenceStatsMock.Object;
            monsterCombatStatsMock.SetupGet(x => x.DefenceStats).Returns(monsterDefenceStats);

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

        [SetUp]
        public void SetUp()
        {
            var actUsageRandomSourceMock = new Mock<ITacticalActUsageRandomSource>();
            actUsageRandomSourceMock.Setup(x => x.RollToHit()).Returns(6);
            _actUsageRandomSource = actUsageRandomSourceMock.Object;

            _perkResolverMock = new Mock<IPerkResolver>();
            _perkResolver = _perkResolverMock.Object;

            var personMock = new Mock<IPerson>();
            _person = personMock.Object;

            var evolutionDataMock = new Mock<IEvolutionData>();
            var evolutionData = evolutionDataMock.Object;
            personMock.SetupGet(x => x.EvolutionData).Returns(evolutionData);

            var actScheme = new TestTacticalActStatsSubScheme
            {
                Offence = new TestTacticalActOffenceSubScheme
                {
                    Type = OffenseType.Tactical,
                    Impact = ImpactType.Kinetic
                }
            };

            var actMock = new Mock<ITacticalAct>();
            actMock.SetupGet(x => x.Stats).Returns(actScheme);
            _act = actMock.Object;
        }
    }
}