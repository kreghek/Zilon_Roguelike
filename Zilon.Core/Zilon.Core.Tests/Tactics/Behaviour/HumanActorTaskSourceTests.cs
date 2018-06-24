using Zilon.Core.Tactics.Behaviour;
using System;
using System.Linq;

using FluentAssertions;

using NUnit.Framework;

using Zilon.Core.Persons;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.TestCommon;
using Moq;
using System.Collections.Generic;
using Zilon.Core.Players;

namespace Zilon.Core.Tactics.Behaviour.Tests
{
    /// <summary>
    /// Тест проверяет, что источник намерений генерирует задачу после указания целевого узла.
    /// По окончанию задачи на перемещение должен выдавать пустые задачи.
    /// </summary>
    [TestFixture()]
    public class HumanActorTaskSourceTests
    {
        [Test()]
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

            var behavourSource = new HumanActorTaskSource(actor);
            behavourSource.IntentMove(finishNode);

            var actorList = CreateActorList(actor);


            // ACT
            // ARRANGE

            // 3 шага одна и та же команда, на 4 шаг - null-комманда
            for (var step = 1; step <= 4; step++)
            {
                var commands = behavourSource.GetActorTasks(map, actorList);

                if (step < 4)
                {
                    commands.Count().Should().Be(1);

                    var factCommand = commands[0] as MoveTask;
                    factCommand.Should().NotBeNull();

                    factCommand.IsComplete.Should().Be(false);


                    foreach (var command in commands)
                    {
                        command.Execute();
                    }

                    actor.Node.Should().Be(expectedPath[step - 1]);
                }
                else
                {
                    commands.Should().BeNull();
                }
            }

            actor.Node.Should().Be(finishNode);
        }

        private static IActor CreateActor(HexNode startNode)
        {
            var playerMock = new Mock<IPlayer>();
            var player = playerMock.Object;

            var actor = new Actor(new Person(), player, startNode);
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
            var finishNode = map.Nodes.Cast<HexNode>().SelectBy(1, 5);

            var expectedPath = new[] {
                map.Nodes.Cast<HexNode>().SelectBy(2, 3),
                map.Nodes.Cast<HexNode>().SelectBy(2, 4),
                finishNode
            };

            var playerMock = new Mock<IPlayer>();
            var player = playerMock.Object;

            var actor = CreateActor(startNode);

            var behaviourSource = new HumanActorTaskSource(actor);

            var actorList = CreateActorList(actor);



            // ACT
            behaviourSource.IntentMove(startNode);



            // ASSERT
            var commands = behaviourSource.GetActorTasks(map, actorList);
            commands.Should().NotBeNullOrEmpty();
            commands[0].Should().BeOfType<MoveTask>();
        }

        private static IActorManager CreateActorList(params IActor[] actors)
        {
            var actorListMock = new Mock<IActorManager>();
            var actorListInner = new List<IActor>(actors);
            actorListMock.Setup(x => x.Actors).Returns(actorListInner);
            var actorList = actorListMock.Object;
            return actorList;
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

            var playerMock = new Mock<IPlayer>();
            var player = playerMock.Object;

            var actor = CreateActor(startNode);

            var behaviourSource = new HumanActorTaskSource(actor);



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

            var playerMock = new Mock<IPlayer>();
            var player = playerMock.Object;

            var actor = CreateActor(startNode);

            var behaviourSource = new HumanActorTaskSource(actor);

            var actorList = CreateActorList(actor);

            // ACT

            // 1. Формируем намерение.
            behaviourSource.IntentMove(finishNode);

            // 2. Ждём, пока команда не отработает.
            var commands = behaviourSource.GetActorTasks(map, actorList);

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
            var factCommands = behaviourSource.GetActorTasks(map, actorList);




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

            var behaviourSource = new HumanActorTaskSource(actor);

            var actorList = CreateActorList(actor);


            // ACT

            // 1. Формируем намерение.
            behaviourSource.IntentMove(finishNode);

            // 2. Продвигаем выполнение текущего намерения. НО НЕ ДО ОКОНЧАНИЯ.
            var commands = behaviourSource.GetActorTasks(map, actorList);

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
            var factCommands = behaviourSource.GetActorTasks(map, actorList);




            // ASSERT
            factCommands.Should().NotBeNullOrEmpty();
        }

        /// <summary>
        /// Тест проверяет, то источник задач возвращает задачу, если указать намерение атаковать.
        /// </summary>
        [Test()]
        public void IntentAttackTest()
        {
            //ARRANGE
            var map = new TestGridGenMap();

            var attackerStartNode = map.Nodes.Cast<HexNode>().SelectBy(3, 3);
            var targetStartNode = map.Nodes.Cast<HexNode>().SelectBy(2, 3);


            var attackerActor = CreateActor(attackerStartNode);
            var targetActor = CreateActor(targetStartNode);


            var taskSource = new HumanActorTaskSource(attackerActor);

            var actorList = CreateActorList(attackerActor, targetActor);


            // ACT
            taskSource.IntentAttack(targetActor);



            // ASSERT
            var tasks = taskSource.GetActorTasks(map, actorList);

            tasks.Should().NotBeNullOrEmpty();
            tasks[0].Should().BeOfType<AttackTask>();
        }
    }
}