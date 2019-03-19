using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Common;
using Zilon.Core.MapGenerators.PrimitiveStyle;
using Zilon.Core.Persons;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Tests.Tactics.Behaviour
{
    [TestFixture]
    public class AttackTaskTests
    {
        private DamageActorTask _attackTask;
        private IActor _actor;
        private IMap _testMap;

        /// <summary>
        /// Тест проверяет, что при атаке, если не мешают стены, не выбрасывается исключение.
        /// </summary>
        [Test]
        public async Task AttackTask_NoWall_NotThrowsInvalidOperationException()
        {
            // Подготовка. Два актёра через клетку. Радиус действия 1-2, достаёт.
            _testMap = await SquareMapFactory.CreateAsync(3);

            var tacticalActMock = new Mock<ITacticalAct>();
            tacticalActMock.SetupGet(x => x.Stats).Returns(new TestTacticalActStatsSubScheme
            {
                Range = new Range<int>(1, 2)
            });
            var tacticalAct = tacticalActMock.Object;

            var actCarrierMock = new Mock<ITacticalActCarrier>();
            actCarrierMock.SetupGet(x => x.Acts)
                .Returns(new[] { tacticalAct });
            var actCarrier = actCarrierMock.Object;

            var personMock = new Mock<IPerson>();
            personMock.SetupGet(x => x.TacticalActCarrier).Returns(actCarrier);
            var person = personMock.Object;

            var actorMock = new Mock<IActor>();
            var actorNode = _testMap.Nodes.OfType<HexNode>().SelectBy(0, 0);
            actorMock.SetupGet(x => x.Node).Returns(actorNode);
            actorMock.SetupGet(x => x.Person).Returns(person);
            actorMock.Setup(x => x.UseAct(It.IsAny<IAttackTarget>(), It.IsAny<ITacticalAct>()))
                .Raises<IAttackTarget, ITacticalAct>(x => x.UsedAct += null, (target1, act1) => new UsedActEventArgs(target1, act1));
            _actor = actorMock.Object;


            var targetMock = new Mock<IActor>();
            var targetNode = _testMap.Nodes.OfType<HexNode>().SelectBy(2, 0);
            targetMock.Setup(x => x.CanBeDamaged()).Returns(true);
            targetMock.SetupGet(x => x.Node).Returns(targetNode);
            var target = targetMock.Object;

            var actServiceMock = new Mock<ITacticalActUsageService>();
            var actService = actServiceMock.Object;


            // Создаём саму команду
            _attackTask = new DamageActorTask(_actor, target, actService);

            Action act = () =>
            {
                // ACT
                _attackTask.Execute();
            };



            // ASSERT
            act.Should().NotThrow<InvalidOperationException>();
        }
    }
}