using Zilon.Core.Tactics.Behaviour;
using System;
using System.Linq;

using FluentAssertions;

using NUnit.Framework;

using Zilon.Core.Persons;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.TestCommon;

namespace Zilon.Core.Tactics.Behaviour.Tests
{
    /// <summary>
    /// Тест проверяет, что источник намерений генерирует намерения после указания точки перемещение.
    /// По окончанию задачи на перемещение должен выдавать пустые намерения.
    /// </summary>
    [TestFixture()]
    public class HumanActorTaskSourceTests
    {
        [Test()]
        public void GetActorTasksTest()
        {
            // ARRANGE
            var map = new TestGridGenMap();

            var startNode = map.Nodes.SelectBy(3, 3);
            var finishNode = map.Nodes.SelectBy(1, 5);

            var expectedPath = new[] {
                map.Nodes.SelectBy(2, 3),
                map.Nodes.SelectBy(2, 4),
                finishNode
            };

            var actor = new Actor(new Person(), startNode);

            var behSource = new HumanActorTaskSource(actor);
            behSource.IntentMove(finishNode);


            // ACT
            // ARRANGE

            // 3 шага одна и та же команда, на 4 шаг - null-комманда
            for (var step = 1; step <= 4; step++)
            {
                var commands = behSource.GetActorTasks(map, new[] { actor });

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

        /// <summary>
        /// Данный методо проверяет, чтобы всегда при выдачи команды на перемещение генерировалась хотя бы одна команда.
        /// </summary>
        [Test()]
        public void AssignMoveToPointCommandTest()
        {
            // ARRANGE

            var map = new TestGridGenMap();

            var startNode = map.Nodes.SelectBy(3, 3);
            var finishNode = map.Nodes.SelectBy(1, 5);

            var expectedPath = new[] {
                map.Nodes.SelectBy(2, 3),
                map.Nodes.SelectBy(2, 4),
                finishNode
            };


            var actor = new Actor(new Person(), startNode);

            var behSource = new HumanActorTaskSource(actor);



            // ACT
            behSource.IntentMove(startNode);



            // ASSERT
            var commands = behSource.GetActorTasks(map, new[] { actor });
            commands.Should().NotBeNullOrEmpty();
            commands[0].Should().BeOfType<MoveTask>();
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

            var startNode = map.Nodes.SelectBy(3, 3);

            var actor = new Actor(new Person(), startNode);

            var behSource = new HumanActorTaskSource(actor);



            // ACT
            Action act = () => { behSource.IntentMove(null); };



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

            var startNode = map.Nodes.SelectBy(3, 3);
            var finishNode = map.Nodes.SelectBy(1, 5);
            var finishNode2 = map.Nodes.SelectBy(3, 2);

            var actor = new Actor(new Person(), startNode);

            var behSource = new HumanActorTaskSource(actor);


            // ACT

            // 1. Формируем намерение.
            behSource.IntentMove(finishNode);

            // 2. Ждём, пока команда не отработает.
            var commands = behSource.GetActorTasks(map, new[] { actor });

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
            behSource.IntentMove(finishNode2);


            // 4. Запрашиваем текущие команды.
            var factCommands = behSource.GetActorTasks(map, new[] { actor });




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

            var startNode = map.Nodes.SelectBy(3, 3);
            var finishNode = map.Nodes.SelectBy(1, 5);
            var finishNode2 = map.Nodes.SelectBy(3, 2);

            var actor = new Actor(new Person(), startNode);

            var behSource = new HumanActorTaskSource(actor);


            // ACT

            // 1. Формируем намерение.
            behSource.IntentMove(finishNode);

            // 2. Продвигаем выполнение текущего намерения. НО НЕ ДО ОКОНЧАНИЯ.
            var commands = behSource.GetActorTasks(map, new[] { actor });

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
            behSource.IntentMove(finishNode2);


            // 4. Запрашиваем текущие команды.
            var factCommands = behSource.GetActorTasks(map, new[] { actor });




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
            var person = new Person { Hp = 1, Damage = 1 };

            var map = new TestGridGenMap();

            var attackerStartNode = map.Nodes.SelectBy(3, 3);
            var targetStartNode = map.Nodes.SelectBy(2, 3);

            var attackerActor = new Actor(person, attackerStartNode);
            var targetActor = new Actor(person, targetStartNode);


            var taskSource = new HumanActorTaskSource(attackerActor);


            // ACT
            taskSource.IntentAttack(targetActor);



            // ASSERT
            var tasks = taskSource.GetActorTasks(map, new[] { attackerActor, targetActor });

            tasks.Should().NotBeNullOrEmpty();
            tasks[0].Should().BeOfType<AttackTask>();
        }
    }
}