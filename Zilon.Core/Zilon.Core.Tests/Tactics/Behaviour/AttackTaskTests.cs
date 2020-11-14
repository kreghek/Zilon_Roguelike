using System;
using System.Threading.Tasks;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Common;
using Zilon.Core.MapGenerators.PrimitiveStyle;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Tests.Tactics.Behaviour
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class AttackTaskTests
    {
        private IActor _actor;
        private AttackTask _attackTask;
        private IMap _testMap;

        /// <summary>
        /// Тест проверяет, что при атаке, если не мешают стены, не выбрасывается исключение.
        /// </summary>
        [Test]
        public async Task AttackTask_NoWall_NotThrowsInvalidOperationException()
        {
            // Подготовка. Два актёра через клетку. Радиус действия 1-2, достаёт.
            _testMap = await SquareMapFactory.CreateAsync(3).ConfigureAwait(false);

            var tacticalActMock = new Mock<ITacticalAct>();
            tacticalActMock.SetupGet(x => x.Stats).Returns(new TestTacticalActStatsSubScheme
            {
                Range = new Range<int>(1, 2)
            });
            var tacticalAct = tacticalActMock.Object;

            var combatActModuleMock = new Mock<ICombatActModule>();
            combatActModuleMock.Setup(x => x.CalcCombatActs())
                .Returns(new[] { tacticalAct });
            var combatActModule = combatActModuleMock.Object;

            var personMock = new Mock<IPerson>();
            personMock.Setup(x => x.GetModule<ICombatActModule>(It.IsAny<string>())).Returns(combatActModule);
            var person = personMock.Object;

            var actorMock = new Mock<IActor>();
            var actorNode = _testMap.Nodes.SelectByHexCoords(0, 0);
            actorMock.SetupGet(x => x.Node).Returns(actorNode);
            actorMock.SetupGet(x => x.Person).Returns(person);
            actorMock.Setup(x => x.UseAct(It.IsAny<IAttackTarget>(), It.IsAny<ITacticalAct>()))
                .Raises<IAttackTarget, ITacticalAct>(x => x.UsedAct += null,
                    (target1, act1) => new UsedActEventArgs(target1, act1));
            _actor = actorMock.Object;

            var targetMock = new Mock<IActor>();
            var targetNode = _testMap.Nodes.SelectByHexCoords(2, 0);
            targetMock.Setup(x => x.CanBeDamaged()).Returns(true);
            targetMock.SetupGet(x => x.Node).Returns(targetNode);
            var target = targetMock.Object;

            var actServiceMock = new Mock<ITacticalActUsageService>();
            var actService = actServiceMock.Object;

            var taskContextMock = new Mock<IActorTaskContext>();
            var taskContext = taskContextMock.Object;

            // Создаём саму команду
            _attackTask = new AttackTask(_actor, taskContext, target, tacticalAct, actService);

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