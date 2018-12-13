using System;
using System.Linq;

using FluentAssertions;

using LightInject;

using Moq;

using NUnit.Framework;

using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common;

//TODO Провести реорганизацию тестов. Потому что они не актуальны после ввода абстракции намерений
namespace Zilon.Core.Tests.Tactics.Behaviour
{
    /// <summary>
    /// Тест проверяет, что источник намерений генерирует задачу после указания целевого узла.
    /// По окончанию задачи на перемещение должен выдавать пустые задачи.
    /// </summary>
    [TestFixture]
    public class HumanActorTaskSourceTests
    {
        private ServiceContainer _container;

        private IHumanActorTaskSource InitTaskSource(IActor currentActor)
        {
            var taskSource = _container.GetInstance<IHumanActorTaskSource>();
            taskSource.SwitchActor(currentActor);
            return taskSource;
        }

        private static IDecisionSource CreateDecisionSource()
        {
            var decisionSourceMock = new Mock<IDecisionSource>();
            var decisionSource = decisionSourceMock.Object;
            return decisionSource;
        }

        private static IActor CreateActor(IMap map, HexNode startNode)
        {
            var playerMock = new Mock<IPlayer>();
            var player = playerMock.Object;

            var personMock = new Mock<IPerson>();
            var person = personMock.Object;

            var actor = new Actor(person, player, startNode);

            map.HoldNode(startNode, actor);

            return actor;
        }

        /// <summary>
        /// Данный метод проверяет, чтобы всегда при выдачи задачи на перемещение генерировалась хотя бы одна задача.
        /// </summary>
        /// <remarks>
        /// Потому что уже команда должна разбираться, что делать, если актёр уже стоит в целевой точке.
        /// </remarks>
        [Test()]
        public void Intent_TargetToStartPoint_GenerateMoveCommand()
        {
            // ARRANGE

            var map = SquareMapFactory.Create(10);

            var startNode = map.Nodes.Cast<HexNode>().SelectBy(3, 3);

            var actor = CreateActor(map, startNode);

            var taskSource = InitTaskSource(actor);

            var moveIntention = new MoveIntention(startNode, map);



            // ACT
            var tasks = SetHumanIntention(actor, taskSource, moveIntention);



            // ASSERT
            tasks.Should().NotBeNullOrEmpty();
            tasks[0].Should().BeOfType<MoveTask>();
        }

        /// <summary>
        /// Тест проверяет, чтобы метод назначения задачи на перемещение проверял аргумент.
        /// Аргумент не должен быть null. Класс поведения не знает, как в этом случае поступать.
        /// </summary>
        /// <remarks>
        /// Для отмены текущего намерения или выполняемой команды используем специальное намерение CancelIntention.
        /// </remarks>
        [Test()]
        public void Intent_SetNullTarget_ThrownException()
        {
            // ARRANGE

            var map = SquareMapFactory.Create(10);

            var startNode = map.Nodes.Cast<HexNode>().SelectBy(3, 3);

            var actor = CreateActor(map, startNode);

            var taskSource = InitTaskSource(actor);



            // ACT
            Action act = () => { taskSource.Intent(null); };



            // ASSERT
            act.Should().Throw<ArgumentException>();
        }

        //TODO Этот тест - это проверка намерения. Сделать отдельный тест.
        /// <summary>
        /// Тест проверяет, что после окончания команды на перемещение и назначения новой команды всё работает корректно.
        /// То есть новая команда возвращается при запросе.
        /// </summary>
        [Test()]
        public void Intent_AssignAfterTaskComplete_NoNullCommand()
        {
            // ARRANGE

            var map = SquareMapFactory.Create(10);

            var startNode = map.Nodes.Cast<HexNode>().SelectBy(3, 3);
            var finishNode = map.Nodes.Cast<HexNode>().SelectBy(1, 5);
            var finishNode2 = map.Nodes.Cast<HexNode>().SelectBy(3, 2);

            var actor = CreateActor(map, startNode);

            var taskSource = InitTaskSource(actor);

            var moveIntention = new MoveIntention(finishNode, map);
            var moveIntention2 = new MoveIntention(finishNode2, map);



            // ACT


            // 1. Ждём, пока задача на перемещение не отработает.
            // В конце текущая задача актёра будет IsComplete.
            var tasks = SetHumanIntention(actor, taskSource, moveIntention);

            for (var i = 0; i < 3; i++)
            {
                foreach (var task in tasks)
                {

                    task.Execute();
                }
            }

            foreach (var task in tasks)
            {
                task.IsComplete.Should().Be(true);
            }


            // 2. Указываем намерение на ещё одну задачу.
            var factTasks = SetHumanIntention(actor, taskSource, moveIntention2);




            // ASSERT
            factTasks.Should().NotBeNullOrEmpty();
        }

        /// <summary>
        /// Тест проверяет, что если указать ещё одно намерение на перемещение, пока предыдущая задача не выполнилась,
        /// то новое намерение отменяет текущее.
        /// </summary>
        [Test()]
        public void Intent_AssignTaskBeforeCurrentTaskComplete_NoNullCommand()
        {
            // ARRANGE

            var map = SquareMapFactory.Create(10);

            var startNode = map.Nodes.Cast<HexNode>().SelectBy(3, 3);
            var finishNode = map.Nodes.Cast<HexNode>().SelectBy(1, 5);
            var finishNode2 = map.Nodes.Cast<HexNode>().SelectBy(3, 2);

            var actor = CreateActor(map, startNode);

            var taskSource = InitTaskSource(actor);

            var moveIntention = new MoveIntention(finishNode, map);
            var moveIntention2 = new MoveIntention(finishNode2, map);



            // ACT

            // 1. Продвигаем выполнение текущего намерения. НО НЕ ДО ОКОНЧАНИЯ.
            var tasks = SetHumanIntention(actor, taskSource, moveIntention);

            for (var i = 0; i < 1; i++)
            {
                foreach (var task in tasks)
                {

                    task.Execute();
                }
            }

            foreach (var task in tasks)
            {
                task.IsComplete.Should().Be(false);
            }


            // 2. Указываем другое намерение до того, как текущая задача на перемещение выполнена до конца.
            var factTasks = SetHumanIntention(actor, taskSource, moveIntention2);



            // ASSERT
            factTasks.Should().NotBeNullOrEmpty();
        }

        private static IActorTask[] SetHumanIntention(IActor actor,
            IHumanActorTaskSource taskSource,
            IIntention intention)
        {
            taskSource.Intent(intention);

            var tasks = taskSource.GetActorTasks(actor);
            return tasks;
        }

        /// <summary>
        /// Тест проверяет, то источник задач возвращает задачу, если указать намерение атаковать.
        /// </summary>
        [Test()]
        public void IntentAttack_SetTarget_ReturnsAttackTask()
        {
            //ARRANGE
            var usageService = _container.GetInstance<ITacticalActUsageService>();

            var map = SquareMapFactory.Create(10);

            var attackerStartNode = map.Nodes.Cast<HexNode>().SelectBy(3, 3);
            var targetStartNode = map.Nodes.Cast<HexNode>().SelectBy(2, 3);


            var attackerActor = CreateActor(map, attackerStartNode);
            var targetActor = CreateActor(map, targetStartNode);

            var taskSource = InitTaskSource(attackerActor);

            var attackIntention = new Intention<AttackTask>(a => new AttackTask(a, targetActor, usageService));



            // ACT
            var tasks = SetHumanIntention(attackerActor, taskSource, attackIntention);



            // ASSERT
            tasks.Should().NotBeNullOrEmpty();
            tasks[0].Should().BeOfType<AttackTask>();
        }

        /// <summary>
        /// Тест проверяет, то источник задач возвращает задачу, если указать намерение открыть контейнер.
        /// </summary>
        [Test()]
        public void IntentOpenContainer_SetContainerAndMethod_ReturnsTask()
        {
            //ARRANGE
            var map = SquareMapFactory.Create(10);

            var startNode = map.Nodes.Cast<HexNode>().SelectBy(0, 0);

            var actor = CreateActor(map, startNode);

            var taskSource = InitTaskSource(actor);

            var containerMock = new Mock<IPropContainer>();
            var container = containerMock.Object;

            var methodMock = new Mock<IOpenContainerMethod>();
            var method = methodMock.Object;

            var intention = new Intention<OpenContainerTask>(a => new OpenContainerTask(a, container, method, map));



            // ACT
            var tasks = SetHumanIntention(actor, taskSource, intention);



            // ASSERT
            tasks.Should().NotBeNullOrEmpty();
            tasks[0].Should().BeOfType<OpenContainerTask>();
        }

        [SetUp]
        public void SetUp()
        {
            _container = new ServiceContainer();

            var decisionSource = CreateDecisionSource();

            var tacticalActUsageService = CreateTacticalActUsageService();

            _container.Register(factory => tacticalActUsageService, new PerContainerLifetime());
            _container.Register(factory => decisionSource, new PerContainerLifetime());
            _container.Register<IHumanActorTaskSource, HumanActorTaskSource>(new PerContainerLifetime());
        }

        private ITacticalActUsageService CreateTacticalActUsageService()
        {
            var tacticalActUsageServiceMock = new Mock<ITacticalActUsageService>();
            var tacticalActUsageService = tacticalActUsageServiceMock.Object;
            return tacticalActUsageService;
        }
    }
}