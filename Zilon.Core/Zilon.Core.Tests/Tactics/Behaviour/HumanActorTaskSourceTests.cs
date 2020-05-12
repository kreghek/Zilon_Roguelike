//using System;
//using System.Linq;
//using System.Threading.Tasks;

//using FluentAssertions;

//using Moq;

//using NUnit.Framework;

//using Zilon.Core.MapGenerators.PrimitiveStyle;
//using Zilon.Core.Persons;
//using Zilon.Core.Players;
//using Zilon.Core.Tactics;
//using Zilon.Core.Tactics.Behaviour;
//using Zilon.Core.Tactics.Spatial;
//using Zilon.Core.Tests.Common;

//namespace Zilon.Core.Tests.Tactics.Behaviour
//{
//    /// <summary>
//    /// Тест проверяет, что источник намерений генерирует задачу после указания целевого узла.
//    /// По окончанию задачи на перемещение должен выдавать пустые задачи.
//    /// </summary>
//    [TestFixture]
//    public class HumanActorTaskSourceTests
//    {
//        /// <summary>
//        /// Тест проверяет, чтобы всегда при выдачи задачи на перемещение генерировалась хотя бы одна задача.
//        /// </summary>
//        /// <remarks>
//        /// Потому что уже команда должна разбираться, что делать, если актёр уже стоит в целевой точке.
//        /// </remarks>
//        [Test]
//        public async Task Intent_TargetToStartPoint_GenerateMoveCommand()
//        {
//            // ARRANGE

//            var map = await SquareMapFactory.CreateAsync(10).ConfigureAwait(false);

//            var startNode = map.Nodes.SelectByHexCoords(3, 3);

//            var actor = CreateActor(map, startNode);

//            var taskSource = InitTaskSource(actor);

//            var moveIntention = new MoveIntention(startNode, map);

//            // ACT
//            var tasks = SetHumanIntention(actor, taskSource, moveIntention);

//            // ASSERT
//            tasks.Should().NotBeNullOrEmpty();
//            tasks[0].Should().BeOfType<MoveTask>();
//        }

//        /// <summary>
//        /// Тест проверяет, чтобы метод назначения задачи на перемещение проверял аргумент.
//        /// Аргумент не должен быть null. Класс поведения не знает, как в этом случае поступать.
//        /// </summary>
//        /// <remarks>
//        /// Для отмены текущего намерения или выполняемой команды используем специальное намерение CancelIntention.
//        /// </remarks>
//        [Test]
//        public async Task Intent_SetNullTarget_ThrownException()
//        {
//            // ARRANGE

//            var map = await SquareMapFactory.CreateAsync(10).ConfigureAwait(false);

//            var startNode = map.Nodes.SelectByHexCoords(3, 3);

//            var actor = CreateActor(map, startNode);

//            var taskSource = InitTaskSource(actor);

//            // ACT
//            Action act = () => { taskSource.Intent(null); };

//            // ASSERT
//            act.Should().Throw<ArgumentException>();
//        }

//        /// <summary>
//        /// Тест проверяет, что после окончания задачи на перемещение
//        /// и назначения новой задачи всё работает корректно.
//        /// То есть новая команда и возвращается при запросе.
//        /// </summary>
//        [Test]
//        public async Task Intent_AssignAfterTaskComplete_NoNullCommand()
//        {
//            // ARRANGE

//            var map = await SquareMapFactory.CreateAsync(10).ConfigureAwait(false);

//            var startNode = map.Nodes.SelectByHexCoords(3, 3);
//            var finishNode = map.Nodes.SelectByHexCoords(1, 5);
//            var finishNode2 = map.Nodes.SelectByHexCoords(3, 2);

//            var actor = CreateActor(map, startNode);

//            var taskSource = InitTaskSource(actor);

//            var moveIntention = new MoveIntention(finishNode, map);
//            var moveIntention2 = new MoveIntention(finishNode2, map);

//            // ACT

//            // 1. Ждём, пока задача на перемещение не отработает.
//            // В конце текущая задача актёра будет IsComplete.
//            var tasks = SetHumanIntention(actor, taskSource, moveIntention);

//            for (var i = 0; i < 3; i++)
//            {
//                foreach (var task in tasks)
//                {
//                    task.Execute();
//                }
//            }

//            foreach (var task in tasks)
//            {
//                task.IsComplete.Should().Be(true);
//            }

//            // 2. Указываем намерение на ещё одну задачу.
//            var factTasks = SetHumanIntention(actor, taskSource, moveIntention2);

//            // ASSERT
//            factTasks.Should().NotBeNullOrEmpty();
//        }

//        /// <summary>
//        /// Тест проверяет, что если указать ещё одно намерение на перемещение, пока предыдущая задача не выполнилась,
//        /// то новое намерение замещает текущее.
//        /// </summary>
//        [Test]
//        public async Task Intent_AssignTaskBeforeCurrentTaskComplete_NoNullCommand()
//        {
//            // ARRANGE

//            var map = await SquareMapFactory.CreateAsync(10).ConfigureAwait(false);

//            var startNode = map.Nodes.SelectByHexCoords(3, 3);
//            var finishNode = map.Nodes.SelectByHexCoords(1, 5);
//            var finishNode2 = map.Nodes.SelectByHexCoords(3, 2);

//            var actor = CreateActor(map, startNode);

//            var taskSource = InitTaskSource(actor);

//            var moveIntention = new MoveIntention(finishNode, map);
//            var moveIntention2 = new MoveIntention(finishNode2, map);

//            // ACT

//            // 1. Продвигаем выполнение текущего намерения. НО НЕ ДО ОКОНЧАНИЯ.
//            var tasks = SetHumanIntention(actor, taskSource, moveIntention);

//            for (var i = 0; i < 1; i++)
//            {
//                foreach (var task in tasks)
//                {
//                    task.Execute();
//                }
//            }

//            foreach (var task in tasks)
//            {
//                task.IsComplete.Should().Be(false);
//            }

//            // 2. Указываем другое намерение до того, как текущая задача на перемещение выполнена до конца.
//            var factTasks = SetHumanIntention(actor, taskSource, moveIntention2);

//            // ASSERT
//            factTasks.Should().NotBeNullOrEmpty();
//        }

//        /// <summary>
//        /// Тест проверяет, то источник задач возвращает задачу, если указать намерение атаковать.
//        /// </summary>
//        [Test]
//        public async Task IntentAttack_SetTarget_ReturnsAttackTask()
//        {
//            //ARRANGE
//            var usageService = CreateTacticalActUsageService();

//            var map = await SquareMapFactory.CreateAsync(10).ConfigureAwait(false);

//            var attackerStartNode = map.Nodes.SelectByHexCoords(3, 3);
//            var targetStartNode = map.Nodes.SelectByHexCoords(2, 3);

//            var attackerActor = CreateActor(map, attackerStartNode);
//            var targetActor = CreateActor(map, targetStartNode);

//            var taskSource = InitTaskSource(attackerActor);

//            var attackIntention = new Intention<AttackTask>(a => new AttackTask(a, targetActor, tacticalAct: null, actService: usageService));

//            // ACT
//            var task = SetHumanIntention(attackerActor, taskSource, attackIntention);

//            // ASSERT
//            task.Should().NotBeNull();
//            task.Should().BeOfType<AttackTask>();
//        }

//        /// <summary>
//        /// Тест проверяет, то источник задач возвращает задачу, если указать намерение открыть контейнер.
//        /// </summary>
//        [Test]
//        public async Task IntentOpenContainer_SetContainerAndMethod_ReturnsTask()
//        {
//            //ARRANGE
//            var map = await SquareMapFactory.CreateAsync(10).ConfigureAwait(false);

//            var startNode = map.Nodes.SelectByHexCoords(0, 0);

//            var actor = CreateActor(map, startNode);

//            var taskSource = InitTaskSource(actor);

//            var containerMock = new Mock<IStaticObject>();
//            var container = containerMock.Object;

//            var methodMock = new Mock<IOpenContainerMethod>();
//            var method = methodMock.Object;

//            var intention = new Intention<OpenContainerTask>(a => new OpenContainerTask(a, container, method, map));

//            // ACT
//            var task = SetHumanIntention(actor, taskSource, intention);

//            // ASSERT
//            task.Should().NotBeNull();
//            task.Should().BeOfType<OpenContainerTask>();
//        }

//        private static ITacticalActUsageService CreateTacticalActUsageService()
//        {
//            var tacticalActUsageServiceMock = new Mock<ITacticalActUsageService>();
//            var tacticalActUsageService = tacticalActUsageServiceMock.Object;
//            return tacticalActUsageService;
//        }

//        private static IHumanActorTaskSource InitTaskSource(IActor currentActor)
//        {
//            var taskSource = new HumanActorTaskSource();
//            taskSource.SwitchActor(currentActor);
//            return taskSource;
//        }

//        private static IActor CreateActor(IMap map, HexNode startNode)
//        {
//            var playerMock = new Mock<IPlayer>();
//            var player = playerMock.Object;

//            var personMock = new Mock<IPerson>();
//            personMock.SetupGet(x => x.PhysicalSize).Returns(PhysicalSize.Size1);
//            var person = personMock.Object;

//            var actor = new Actor(person, player, startNode);

//            map.HoldNode(startNode, actor);

//            return actor;
//        }

//        private static IActorTask SetHumanIntention(IActor actor,
//            IHumanActorTaskSource taskSource,
//            IIntention intention)
//        {
//            taskSource.Intent(intention);

//            var task = taskSource.GetActorTask(actor);
//            return task;
//        }
//    }
//}