using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Players;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.TestCommon;

namespace Zilon.Core.Tactics.Behaviour.Bots.Tests
{
    /// <summary>
    /// Тесты для проверки корректности обхода точек одним актёром.
    /// </summary>
    [TestFixture]
    public class PatrolLogicOneActorBypassTests
    {
        private const int _expectedIdleDuration = 1;

        private IMapNode _factActorNode;
        private IMap _map;
        private IPlayer _player;
        private IActor _actor;
        private IPatrolRoute _patrolRoute;
        private IActorManager _actorList;
        private IDecisionSource _decisionSource;

        [SetUp]
        public void SetUp()
        {
            _map = new TestGridGenMap();


            var playerMock = new Mock<IPlayer>();
            _player = playerMock.Object;


            var actorMock = new Mock<IActor>();
            actorMock.SetupGet(x => x.Node).Returns(() => _factActorNode);
            actorMock.Setup(x => x.MoveToNode(It.IsAny<IMapNode>()))
                .Callback<IMapNode>(node => _factActorNode = node);
            actorMock.SetupGet(x => x.Owner).Returns(_player);
            _actor = actorMock.Object;


            var patrolRouteMock = new Mock<IPatrolRoute>();
            var routePoints = new IMapNode[] {
                _map.Nodes.OfType<HexNode>().SelectBy(1, 1),
                _map.Nodes.OfType<HexNode>().SelectBy(5, 3),
                _map.Nodes.OfType<HexNode>().SelectBy(3, 5)
            };
            patrolRouteMock.SetupGet(x => x.Points).Returns(routePoints);
            _patrolRoute = patrolRouteMock.Object;


            var actors = new List<IActor> { _actor };
            var actorListMock = new Mock<IActorManager>();
            actorListMock.SetupGet(x => x.Actors).Returns(actors);
            _actorList = actorListMock.Object;


            var decisionSourceMock = new Mock<IDecisionSource>();
            decisionSourceMock.Setup(x => x.SelectIdleDuration(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(_expectedIdleDuration);
            _decisionSource = decisionSourceMock.Object;
        }

        /// <summary>
        /// Тест проверяет, что актёр, следуемый логике патрулирования будет
        /// корректно обходить ключевые точки.
        /// В точка должен быть простой на 1 ход.
        /// Изначально актёр начинает патрулирование с первой точки обхода.
        /// </summary>
        [Test]
        public void GetCurrentTask_StartOnFirstPoint_ActorWalkThroughRount()
        {
            // ARRANGE


            _factActorNode = _map.Nodes.OfType<HexNode>().SelectBy(1, 1);

            var expectedActorPositions = new IMapNode[] {
                _map.Nodes.OfType<HexNode>().SelectBy(2, 2),
                _map.Nodes.OfType<HexNode>().SelectBy(2, 3),
                _map.Nodes.OfType<HexNode>().SelectBy(3, 3),
                _map.Nodes.OfType<HexNode>().SelectBy(4, 3),
                _map.Nodes.OfType<HexNode>().SelectBy(5, 3),

                _map.Nodes.OfType<HexNode>().SelectBy(5, 3),

                _map.Nodes.OfType<HexNode>().SelectBy(4, 3),
                _map.Nodes.OfType<HexNode>().SelectBy(4, 4),
                _map.Nodes.OfType<HexNode>().SelectBy(3, 5),

                _map.Nodes.OfType<HexNode>().SelectBy(3, 5),

                _map.Nodes.OfType<HexNode>().SelectBy(3, 4),
                _map.Nodes.OfType<HexNode>().SelectBy(2, 3),
                _map.Nodes.OfType<HexNode>().SelectBy(2, 2),
                _map.Nodes.OfType<HexNode>().SelectBy(1, 1),

                _map.Nodes.OfType<HexNode>().SelectBy(1, 1),
            };


            var logic = new PatrolLogic(_actor, _patrolRoute, _map, _actorList, _decisionSource);



            // ACT
            for (var round = 0; round < expectedActorPositions.Count() + 1; round++)
            {
                var task = logic.GetCurrentTask();


                // ASSERT
                task.Should().NotBeNull();
                switch (round)
                {
                    case 5:
                    case 9:
                    case 14:
                        task.Should().BeOfType<IdleTask>($"На {round} итерации ожидается задача на простой.");
                        break;

                    default:
                        task.Should().BeOfType<MoveTask>($"На {round} итерации ожидается задача на перемещение.");
                        break;
                }

                task.Execute();

                if (round < expectedActorPositions.Count())
                {
                    _factActorNode.Should().Be(expectedActorPositions[round],
                        $"На {round} итерации неожиданные координаты актёра.");
                }
                else
                {
                    _factActorNode.Should().Be(expectedActorPositions[0],
                        $"На {round} итерации актёр должен начать маршрут заново.");
                }
            }

        }


        /// <summary>
        /// Тест проверяет, что актёр, следуемый логике патрулирования будет
        /// корректно обходить ключевые точки.
        /// В точка должен быть простой на 1 ход.
        /// Изначально актёр начинает патрулирование в стороне от маршрута патрулирования.
        /// Ожидается, что актёр в первую очередь посетит ближайшую точку патрулирования
        /// и продолжит обход в порядке точек патрулирования.
        /// </summary>
        [Test]
        public void GetCurrentTask_StartOnSideOnRoute_ActorWalkThroughRount()
        {
            // ARRANGE


            _factActorNode = _map.Nodes.OfType<HexNode>().SelectBy(0, 0);

            var logic = new PatrolLogic(_actor, _patrolRoute, _map, _actorList, _decisionSource);

            const int expectedStepsToPatrolPoint_1_1 = 2;
            var expectedNode = _patrolRoute.Points.First();



            // ACT
            for (var round = 0; round < expectedStepsToPatrolPoint_1_1; round++)
            {
                var task = logic.GetCurrentTask();

                task.Execute();
            }



            // ASSERT
            var factHexNode = (HexNode)_factActorNode;
            var expectedHexNode = (HexNode)expectedNode;
            factHexNode.OffsetX.Should().Be(expectedHexNode.OffsetX);
            factHexNode.OffsetY.Should().Be(expectedHexNode.OffsetY);
        }
    }
}