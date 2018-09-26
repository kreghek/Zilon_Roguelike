using NUnit.Framework;
using Zilon.Core.Tactics.Behaviour.Bots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common;
using Moq;
using Zilon.Core.Tactics;
using Zilon.Core.Players;
using FluentAssertions;

namespace Zilon.Core.Tests.Tactics.Behaviour.Bots
{
    [TestFixture()]
    public class RoamingLogicTests
    {
        [Test()]
        public void GetCurrentTaskTest()
        {
            Assert.Fail();
        }

        private const int _expectedIdleDuration = 1;

        private IMapNode _factActorNode;
        private IMapNode _factIntruderNode;
        private IMap _map;
        private IPlayer _player;
        private IActor _actor;
        private IActorManager _actorList;
        private IDecisionSource _decisionSource;
        private IActor _intruder;

        [SetUp]
        public void SetUp()
        {
            _map = new TestGridGenMap();


            var playerMock = new Mock<IPlayer>();
            _player = playerMock.Object;

            var enemyPlayerMock = new Mock<IPlayer>();
            var enemyPlayer = enemyPlayerMock.Object;


            var actorMock = new Mock<IActor>();
            actorMock.SetupGet(x => x.Node).Returns(() => _factActorNode);
            actorMock.Setup(x => x.MoveToNode(It.IsAny<IMapNode>()))
                .Callback<IMapNode>(node => _factActorNode = node);
            actorMock.SetupGet(x => x.Owner).Returns(_player);
            _actor = actorMock.Object;

            var intruderMock = new Mock<IActor>();
            intruderMock.SetupGet(x => x.Owner).Returns(enemyPlayer);
            intruderMock.SetupGet(x => x.Node).Returns(() => _factIntruderNode);
            intruderMock.Setup(x => x.MoveToNode(It.IsAny<IMapNode>()))
                .Callback<IMapNode>(node => _factIntruderNode = node);
            _intruder = intruderMock.Object;

            var intruderStateMock = new Mock<IActorState>();
            intruderStateMock.SetupGet(x => x.IsDead).Returns(false);
            var intruderState = intruderStateMock.Object;
            intruderMock.SetupGet(x => x.State).Returns(intruderState);

            var actors = new List<IActor> { _actor, _intruder };
            var actorListMock = new Mock<IActorManager>();
            actorListMock.SetupGet(x => x.Items).Returns(actors);
            _actorList = actorListMock.Object;

            var decisionSourceMock = new Mock<IDecisionSource>();
            decisionSourceMock.Setup(x => x.SelectIdleDuration(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(_expectedIdleDuration);
            _decisionSource = decisionSourceMock.Object;
        }

        /// <summary>
        /// Тест проверяет, что актёр, следуемый логике произвольного брожения будет
        /// приближаться к противнику, если тот в зоне обнаружения.
        /// В точка должен быть простой на 1 ход.
        /// </summary>
        [Test]
        public void GetCurrentTask_IntruderNear_ActorWalkToIntruder()
        {
            // ARRANGE

            var expectedActorNode = _map.Nodes.OfType<HexNode>().SelectBy(2, 1);

            _factActorNode = _map.Nodes.OfType<HexNode>().SelectBy(1, 1);

            _map.HoldNode(_factActorNode, _actor);

            _factIntruderNode = _map.Nodes.OfType<HexNode>().SelectBy(3, 1);
            _map.HoldNode(_factIntruderNode, _intruder);



            var tacticalActUsageService = CreateTacticalActUsageService();

            var logic = new RoamingLogic(_actor,
                _map,
                _actorList,
                _decisionSource,
                tacticalActUsageService);



            // ACT

            var task = logic.GetCurrentTask();
            task.Execute();



            // ASSERT
            _factActorNode.Should().Be(expectedActorNode);
        }

        private ITacticalActUsageService CreateTacticalActUsageService()
        {
            var tacticalActUsageServiceMock = new Mock<ITacticalActUsageService>();
            var tacticalActUsageService = tacticalActUsageServiceMock.Object;
            return tacticalActUsageService;
        }
    }
}