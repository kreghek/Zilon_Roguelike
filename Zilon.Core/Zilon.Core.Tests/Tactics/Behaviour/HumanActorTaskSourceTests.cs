using Zilon.Core.Tactics.Behaviour;
using System;
using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.TestCommon;

// ReSharper disable once CheckNamespace
namespace Zilon.Core.Tactics.Behaviour.Tests
{
    /// <summary>
    /// Тест проверяет, что источник намерений генерирует задачу после указания целевого узла.
    /// По окончанию задачи на перемещение должен выдавать пустые задачи.
    /// </summary>
    [TestFixture]
    public class HumanActorTaskSourceTests
    {
        [Test]
        public void GetActorTasksTest()
        {
            // ARRANGE
            var map = new TestGridGenMap();

            var startNode = map.Nodes.Cast<HexNode>().SelectBy(3, 3);
            var finishNode = map.Nodes.Cast<HexNode>().SelectBy(1, 5);

            var expectedPath = new[] {
                map.Nodes.Cast<HexNode>().SelectBy(2, 3),
                map.Nodes.Cast<HexNode>().SelectBy(2, 4),
                finishNode
            };

            var actor = CreateActor(startNode);

            var decisionSource = CreateDecisionSource();

            var tacticalActUsageService = CreateTacticalActUsageService();

            var behavourSource = new HumanActorTaskSource(actor, decisionSource, tacticalActUsageService);
            behavourSource.IntentMove(finishNode);

            var actorManager = CreateActorManager(actor);


            // ACT
            // ARRANGE

            // 3 шага одна и та же команда, на 4 шаг - null-комманда
            for (var step = 1; step <= 4; step++)
            {
                var commands = behavourSource.GetActorTasks(map, actorManager);

                if (step < 4)
                {
                    commands.Length.Should().Be(1);

                    var factCommand = commands[0] as MoveTask;
                    factCommand.Should().NotBeNull();

                    factCommand?.IsComplete.Should().Be(false);


                    foreach (var command in commands)
                    {
                        command.Execute();
                    }

                    actor.Node.Should().Be(expectedPath[step - 1]);
                }
                else
                {
                    commands.Should().BeEmpty();
                }
            }

            actor.Node.Should().Be(finishNode);
        }

        private static IDecisionSource CreateDecisionSource()
        {
            var decisionSourceMock = new Mock<IDecisionSource>();
            var decisionSource = decisionSourceMock.Object;
            return decisionSource;
        }

        private static IActor CreateActor(HexNode startNode)
        {
            var playerMock = new Mock<IPlayer>();
            var player = playerMock.Object;

            var personMock = new Mock<IPerson>();
            var person = personMock.Object;

            var actor = new Actor(person, player, startNode);
            return actor;
        }

        /// <summary>
        /// Данный методо проверяет, чтобы всегда при выдачи команды на перемещение генерировалась хотя бы одна команда.
        /// </summary>
        [Test()]
        public void AssignMoveToPointCommandTest()
        {
            // ARRANGE

            var map = new TestGridGenMap();

            var startNode = map.Nodes.Cast<HexNode>().SelectBy(3, 3);

            var decisionSource = CreateDecisionSource();

            var actor = CreateActor(startNode);

            var tacticalActUsageService = CreateTacticalActUsageService();

            var behaviourSource = new HumanActorTaskSource(actor, decisionSource, tacticalActUsageService);

            var actorManager = CreateActorManager(actor);



            // ACT
            behaviourSource.IntentMove(startNode);



            // ASSERT
            var commands = behaviourSource.GetActorTasks(map, actorManager);
            commands.Should().NotBeNullOrEmpty();
            commands[0].Should().BeOfType<MoveTask>();
        }

        private static IActorManager CreateActorManager(params IActor[] actors)
        {
            var actorManagerMock = new Mock<IActorManager>();
            var actorListInner = new List<IActor>(actors);
            actorManagerMock.Setup(x => x.Actors).Returns(actorListInner);
            var actorManager = actorManagerMock.Object;
            return actorManager;
        }

        /// <summary>
        /// Тест проверяет, чтобы метод назначения задачи на перемещение проверял аргумент.
        /// Аргумент не должен быть null. Класс поведенения не знает, как в этом случае поступать.
        /// </summary>
        [Test()]
        public void AssignMoveToPointCommand_SetNullTarget_Exception()
        {
            // ARRANGE

            var map = new TestGridGenMap();

            var startNode = map.Nodes.Cast<HexNode>().SelectBy(3, 3);

            var decisionSource = CreateDecisionSource();

            var actor = CreateActor(startNode);

            var tacticalActUsageService = CreateTacticalActUsageService();

            var behaviourSource = new HumanActorTaskSource(actor, decisionSource, tacticalActUsageService);



            // ACT
            Action act = () => { behaviourSource.IntentMove(null); };



            // ASSERT
            act.Should().Throw<ArgumentException>();
        }

        /// <summary>
        /// Тест проверяет, после окончания команды на перемещение и назнаения новой команды всё рабоает корректно.
        /// То есть новая команда возвращается при запросе.
        /// </summary>
        [Test()]
        public void AssignMoveToPointCommand_AssignAfterTaskComplete_NoNullCommand()
        {
            // ARRANGE

            var map = new TestGridGenMap();

            var startNode = map.Nodes.Cast<HexNode>().SelectBy(3, 3);
            var finishNode = map.Nodes.Cast<HexNode>().SelectBy(1, 5);
            var finishNode2 = map.Nodes.Cast<HexNode>().SelectBy(3, 2);

            var decisionSource = CreateDecisionSource();

            var actor = CreateActor(startNode);

            var tacticalActUsageService = CreateTacticalActUsageService();

            var behaviourSource = new HumanActorTaskSource(actor, decisionSource, tacticalActUsageService);

            var actorManager = CreateActorManager(actor);

            // ACT

            // 1. Формируем намерение.
            behaviourSource.IntentMove(finishNode);

            // 2. Ждём, пока команда не отработает.
            var commands = behaviourSource.GetActorTasks(map, actorManager);

            for (var i = 0; i < 3; i++)
            {
                foreach (var command in commands)
                {

                    command.Execute();
                }
            }

            foreach (var command in commands)
            {
                command.IsComplete.Should().Be(true);
            }

            // 3. Формируем ещё одно намерение.
            behaviourSource.IntentMove(finishNode2);


            // 4. Запрашиваем текущие команды.
            var factCommands = behaviourSource.GetActorTasks(map, actorManager);




            // ASSERT
            factCommands.Should().NotBeNullOrEmpty();
        }

        /// <summary>
        /// Тест проверяет, что если указать ещё одно намерение на перемещение, пока предыдущая задача не выполнилась,
        /// то новое намерение отменяет текущее.
        /// </summary>
        [Test()]
        public void AssignMoveToPointCommand_AssignTaskBeforeCurrentTaskComplete_NoNullCommand()
        {
            // ARRANGE

            var map = new TestGridGenMap();

            var startNode = map.Nodes.Cast<HexNode>().SelectBy(3, 3);
            var finishNode = map.Nodes.Cast<HexNode>().SelectBy(1, 5);
            var finishNode2 = map.Nodes.Cast<HexNode>().SelectBy(3, 2);

            var actor = CreateActor(startNode);

            var decisionSource = CreateDecisionSource();

            var tacticalActUsageService = CreateTacticalActUsageService();

            var behaviourSource = new HumanActorTaskSource(actor, decisionSource, tacticalActUsageService);

            var actorManager = CreateActorManager(actor);


            // ACT

            // 1. Формируем намерение.
            behaviourSource.IntentMove(finishNode);

            // 2. Продвигаем выполнение текущего намерения. НО НЕ ДО ОКОНЧАНИЯ.
            var commands = behaviourSource.GetActorTasks(map, actorManager);

            for (var i = 0; i < 1; i++)
            {
                foreach (var command in commands)
                {

                    command.Execute();
                }
            }

            foreach (var command in commands)
            {
                command.IsComplete.Should().Be(false);
            }

            // 3. Формируем ещё одно намерение.
            behaviourSource.IntentMove(finishNode2);


            // 4. Запрашиваем текущие команды.
            var factCommands = behaviourSource.GetActorTasks(map, actorManager);




            // ASSERT
            factCommands.Should().NotBeNullOrEmpty();
        }

        /// <summary>
        /// Тест проверяет, то источник задач возвращает задачу, если указать намерение атаковать.
        /// </summary>
        [Test()]
        public void IntentAttack_SetTarget_ReturnsAttackTask()
        {
            //ARRANGE
            var map = new TestGridGenMap();

            var attackerStartNode = map.Nodes.Cast<HexNode>().SelectBy(3, 3);
            var targetStartNode = map.Nodes.Cast<HexNode>().SelectBy(2, 3);


            var attackerActor = CreateActor(attackerStartNode);
            var targetActor = CreateActor(targetStartNode);

            var decisionSource = CreateDecisionSource();

            var tacticalActUsageService = CreateTacticalActUsageService();

            var behaviourSource = new HumanActorTaskSource(attackerActor, decisionSource, tacticalActUsageService);

            var actorManager = CreateActorManager(attackerActor, targetActor);


            // ACT
            behaviourSource.IntentAttack(targetActor);



            // ASSERT
            var tasks = behaviourSource.GetActorTasks(map, actorManager);

            tasks.Should().NotBeNullOrEmpty();
            tasks[0].Should().BeOfType<AttackTask>();
        }

        [Test()]
        public void IntentOpenContainer_SetContainerAndMethod_ReturnsTask()
        {
            //ARRANGE
            var map = new TestGridGenMap();

            var startNode = map.Nodes.Cast<HexNode>().SelectBy(0, 0);

            var actor = CreateActor(startNode);

            var decisionSource = CreateDecisionSource();

            var tacticalActUsageService = CreateTacticalActUsageService();

            var behaviourSource = new HumanActorTaskSource(actor, decisionSource, tacticalActUsageService);

            var actorManager = CreateActorManager(actor);

            var containerMock = new Mock<IPropContainer>();
            var container = containerMock.Object;

            var methodMock = new Mock<IOpenContainerMethod>();
            var method = methodMock.Object;

            // ACT
            behaviourSource.IntentOpenContainer(container, method);



            // ASSERT
            var tasks = behaviourSource.GetActorTasks(map, actorManager);

            tasks.Should().NotBeNullOrEmpty();
            tasks[0].Should().BeOfType<OpenContainerTask>();
        }

        private ITacticalActUsageService CreateTacticalActUsageService()
        {
            var tacticalActUsageServiceMock = new Mock<ITacticalActUsageService>();
            var tacticalActUsageService = tacticalActUsageServiceMock.Object;
            return tacticalActUsageService;
        }
    }
}