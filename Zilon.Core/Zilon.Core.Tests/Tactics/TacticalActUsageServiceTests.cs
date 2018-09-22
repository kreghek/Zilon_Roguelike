using System;
using FluentAssertions;
using Moq;

using NUnit.Framework;
using Zilon.Core.Common;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Tests
{
    [TestFixture]
    public class TacticalActUsageServiceTests
    {
        /// <summary>
        /// Тест проверяет, что сервис использования действий корректно наносит урон целевому монстру.
        /// </summary>
        [Test]
        public void UseOn_MonsterHitByAct_MonsterTakesDamage()
        {
            // ARRANGE
            var actUsageRandomSourceMock = new Mock<IActUsageRandomSource>();
            var actUsageRandomSource = actUsageRandomSourceMock.Object;

            var perkResolverMock = new Mock<IPerkResolver>();
            var perkResolver = perkResolverMock.Object;


            var actUsageService = new TacticalActUsageService(actUsageRandomSource, perkResolver);

            var actorMock = new Mock<IActor>();
            actorMock.SetupGet(x => x.Node).Returns(new HexNode(0, 0));
            var actor = actorMock.Object;

            var monsterMock = new Mock<IActor>();
            monsterMock.SetupGet(x => x.Node).Returns(new HexNode(1, 0));
            var monster = monsterMock.Object;

            var monsterStateMock = new Mock<IActorState>();
            var monsterState = monsterStateMock.Object;
            monsterMock.SetupGet(x => x.State).Returns(monsterState);

            var actMock = new Mock<ITacticalAct>();
            actMock.SetupGet(x => x.Stats).Returns(new TacticalActStatsSubScheme { Range = new Range<int>(1, 1) });
            var act = actMock.Object;



            // ACT
            actUsageService.UseOn(actor, monster, act);



            // ASSERT
            monsterMock.Verify(x => x.TakeDamage(It.IsAny<float>()), Times.Once);
        }
    }
}