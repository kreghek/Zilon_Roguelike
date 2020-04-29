using System;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using JetBrains.Annotations;

using Moq;

using NUnit.Framework;

using Zilon.Core.Common;
using Zilon.Core.Components;
using Zilon.Core.Graphs;
using Zilon.Core.MapGenerators.PrimitiveStyle;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Tests.Tactics
{
    [TestFixture]
    public class TacticalActUsageServiceTests
    {
        private ITacticalActUsageRandomSource _actUsageRandomSource;
        private ITacticalAct _act;
        private IPerson _person;
        private ISectorManager _sectorManager;

        /// <summary>
        /// Тест проверяет, что при выстреле изымаются патроны из инвентаря.
        /// </summary>
        [Test]
        public void UseOn_DecreaseBullets_BulletRemovedFromInventory()
        {
            // ARRANGE

            var handlerSelector = CreateEmptyHandlerSelector();

            var actUsageService = new TacticalActUsageService(
                _actUsageRandomSource,
                _sectorManager,
                handlerSelector);

            var personMock = new Mock<IPerson>();
            var person = personMock.Object;

            var actorMock = new Mock<IActor>();
            actorMock.SetupGet(x => x.Node).Returns(new HexNode(0, 0));
            actorMock.SetupGet(x => x.Person).Returns(person);
            var actor = actorMock.Object;

            var monsterMock = CreateOnHitMonsterMock();
            var monster = monsterMock.Object;

            var actStatsSubScheme = new TestTacticalActStatsSubScheme
            {
                Offence = new TestTacticalActOffenceSubScheme
                {
                    Type = OffenseType.Tactical,
                    Impact = ImpactType.Kinetic,
                    ApRank = 10
                }
            };

            var actConstrainsSubScheme = new TestTacticalActConstrainsSubScheme
            {
                PropResourceType = "7-62",
                PropResourceCount = 1
            };

            var inventory = new InventoryModule();
            var bulletScheme = new TestPropScheme
            {
                Sid = "bullet-7-62",
                Bullet = new TestPropBulletSubScheme
                {
                    Caliber = "7-62"
                }
            };
            inventory.Add(new Resource(bulletScheme, 10));
            personMock.Setup(x => x.GetModule<IInventoryModule>(It.IsAny<string>())).Returns(inventory);

            var actMock = new Mock<ITacticalAct>();
            actMock.SetupGet(x => x.Stats).Returns(actStatsSubScheme);
            actMock.SetupGet(x => x.Constrains).Returns(actConstrainsSubScheme);
            var shootAct = actMock.Object;

            // ACT
            var usedActs = new UsedTacticalActs(new[] { shootAct });
            actUsageService.UseOn(actor, monster, usedActs);

            // ASSERT
            var bullets = inventory.CalcActualItems().Single(x => x.Scheme.Sid == "bullet-7-62") as Resource;
            bullets.Count.Should().Be(9);
        }

        /// <summary>
        /// Тест проверяет, что при атаке сквозь стены выбрасывается исключение.
        /// </summary>
        [Test]
        public void UseOn_Wall_ThrowsUsageThroughtWallException()
        {
            // ARRANGE

            var sectorManager = CreateSectorManagerWithWall();

            var handlerSelector = CreateEmptyHandlerSelector();

            var actUsageService = new TacticalActUsageService(
                _actUsageRandomSource,
                sectorManager,
                handlerSelector);

            var actorMock = new Mock<IActor>();
            actorMock.SetupGet(x => x.Node).Returns(new HexNode(0, 0));
            actorMock.SetupGet(x => x.Person).Returns(_person);
            var actor = actorMock.Object;

            var monsterMock = CreateMonsterMock();
            var monster = monsterMock.Object;

            // ACT
            var usedActs = new UsedTacticalActs(new[] { _act });

            Action act = () =>
            {
                actUsageService.UseOn(actor, monster, usedActs);
            };

            // ASSERT
            act.Should().Throw<UsageThroughtWallException>();
        }

        /// <summary>
        /// Тест проверяет, что при атаке вызывается событие использования действия у актёра..
        /// </summary>
        [Test]
        public void UseOn_Attack_RaiseUsedAct()
        {
            // ARRANGE

            var handlerSelector = CreateEmptyHandlerSelector();

            var actUsageService = new TacticalActUsageService(
                _actUsageRandomSource,
                _sectorManager,
                handlerSelector);

            var actorMock = new Mock<IActor>();
            actorMock.SetupGet(x => x.Node).Returns(new HexNode(0, 0));
            actorMock.SetupGet(x => x.Person).Returns(_person);
            actorMock.Setup(x => x.UseAct(It.IsAny<IAttackTarget>(), It.IsAny<ITacticalAct>()))
                .Raises<IAttackTarget, ITacticalAct>(x => x.UsedAct += null, (target1, act1) => new UsedActEventArgs(target1, act1));
            var actor = actorMock.Object;

            var monsterMock = CreateMonsterMock();
            var monster = monsterMock.Object;

            var usedActs = new UsedTacticalActs(new[] { _act });

            using var monitor = actor.Monitor();

            // ACT
            actUsageService.UseOn(actor, monster, usedActs);

            // ASSERT
            monitor.Should().Raise(nameof(IActor.UsedAct));
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

        [SetUp]
        public async Task SetUpAsync()
        {
            var actUsageRandomSourceMock = new Mock<ITacticalActUsageRandomSource>();
            actUsageRandomSourceMock.Setup(x => x.RollToHit(It.IsAny<Roll>())).Returns(6);
            actUsageRandomSourceMock.Setup(x => x.RollEfficient(It.IsAny<Roll>())).Returns(1);
            _actUsageRandomSource = actUsageRandomSourceMock.Object;

            var personMock = new Mock<IPerson>();
            _person = personMock.Object;

            var evolutionModuleMock = new Mock<IEvolutionModule>();
            var evolutionModule = evolutionModuleMock.Object;
            personMock.Setup(x => x.GetModule<IEvolutionModule>(It.IsAny<string>())).Returns(evolutionModule);

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
            _act = actMock.Object;

            var sectorManagerMock = new Mock<ISectorManager>();
            var sectorManager = sectorManagerMock.Object;

            var map = await SquareMapFactory.CreateAsync(3).ConfigureAwait(false);
            var sectorMock = new Mock<ISector>();
            sectorMock.SetupGet(x => x.Map).Returns(map);
            var sector = sectorMock.Object;
            sectorManagerMock.SetupGet(x => x.CurrentSector).Returns(sector);

            _sectorManager = sectorManager;
        }

        private ISectorManager CreateSectorManagerWithWall()
        {
            var sectorManagerMock = new Mock<ISectorManager>();
            var sectorManager = sectorManagerMock.Object;

            var mapMock = new Mock<ISectorMap>();
            mapMock.Setup(x => x.TargetIsOnLine(It.IsAny<IGraphNode>(), It.IsAny<IGraphNode>()))
                .Returns(false);
            mapMock.Setup(x => x.DistanceBetween(It.IsAny<IGraphNode>(), It.IsAny<IGraphNode>()))
                .Returns(1);
            var map = mapMock.Object;

            var sectorMock = new Mock<ISector>();
            sectorMock.SetupGet(x => x.Map).Returns(map);
            var sector = sectorMock.Object;
            sectorManagerMock.SetupGet(x => x.CurrentSector).Returns(sector);
            return sectorManager;
        }

        private static IActUsageHandlerSelector CreateEmptyHandlerSelector()
        {
            var handlerMock = new Mock<IActUsageHandler>();
            var handler = handlerMock.Object;

            var handlerSelectorMock = new Mock<IActUsageHandlerSelector>();
            handlerSelectorMock.Setup(x => x.GetHandler(It.IsAny<IAttackTarget>())).Returns(handler);
            var handlerSelector = handlerSelectorMock.Object;
            return handlerSelector;
        }
    }
}