using System.Linq;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Common;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.TestCommon;

namespace Zilon.Core.Tactics.Behaviour.Tests
{
    [TestFixture()]
    public class AttackTaskTests
    {
        /// <summary>
        /// Тест проверяет, что при атаке вызывается событие использования действия у актёра.
        /// </summary>
        [Test()]
        public void AttackTaskTest()
        {
            // ARRANGE
            // Подготовка. Два актёра через клетку. Радиус действия 1-2, достаёт.
            var testMap = new TestGridGenMap(3);

            var actMock = new Mock<ITacticalAct>();
            actMock.SetupGet(x => x.Stats).Returns(new TacticalActStatsSubScheme
            {
                Range = new Range<int>(1, 2)
            });
            var act = actMock.Object;

            var actCarrierMock = new Mock<ITacticalActCarrier>();
            actCarrierMock.SetupGet(x => x.Acts)
                .Returns(new ITacticalAct[] { act });
            var actCarrier = actCarrierMock.Object;

            var personMock = new Mock<IPerson>();
            personMock.SetupGet(x => x.TacticalActCarrier).Returns(actCarrier);
            var person = personMock.Object;

            var actorMock = new Mock<IActor>();
            var actorNode = testMap.Nodes.OfType<HexNode>().SelectBy(0, 0);
            actorMock.SetupGet(x => x.Node).Returns(actorNode);
            actorMock.SetupGet(x => x.Person).Returns(person);
            actorMock.Setup(x => x.UseAct(It.IsAny<IAttackTarget>(), It.IsAny<ITacticalAct>()))
                .Raises<IAttackTarget, ITacticalAct>(x => x.UsedAct += null, (target1, act1) => new UsedActEventArgs(target1, act1));
            var actor = actorMock.Object;


            var targetMock = new Mock<IActor>();
            var targetNode = testMap.Nodes.OfType<HexNode>().SelectBy(2, 0);
            targetMock.Setup(x => x.CanBeDamaged()).Returns(true);
            targetMock.SetupGet(x => x.Node).Returns(targetNode);
            var target = targetMock.Object;

            var actServiceMock = new Mock<ITacticalActUsageService>();
            var actService = actServiceMock.Object;


            // Создаём саму команду
            var attackTask = new AttackTask(actor, target, actService);


            using (var monitor = actor.Monitor())
            {
                // ACT
                attackTask.Execute();



                // ASSERT
                monitor.Should().Raise(nameof(IActor.UsedAct));
            }
        }
    }
}