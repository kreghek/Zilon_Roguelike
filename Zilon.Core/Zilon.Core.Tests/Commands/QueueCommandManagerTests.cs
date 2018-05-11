using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Zilon.Core.Commands.Tests
{
    [TestFixture()]
    public class QueueCommandManagerTests
    {
        /// <summary>
        /// 1. В системе есть набор команд, размещённых в менеджере за одну итерацию.
        /// 2. Извлекаем все команды.
        /// 3. Команды должны быть такими, какими их поместили в порядке очереди.
        /// </summary>
        /// <param name="commands"></param>
        [Test()]
        [TestCaseSource(typeof(QuequCommandManagerTestCaseGenerator),
            nameof(QuequCommandManagerTestCaseGenerator.PopTestCases))]
        public void Pop_FakeCommands_AllCommandsExtracted(ICommand[] commands)
        {
            // ARRANGE
            var commandManager = new QueueCommandManager();
            foreach (var command in commands)
            {
                commandManager.Push(command);
            }


            // ACT
            for (var i = 0; i < commands.Length; i++)
            {
                var factCommand = commandManager.Pop();


                // ASSERT
                factCommand.Should().Be(commands[i]);
            }
        }

        /// <summary>
        /// 1. В системе выполнили две команды с промежуточной пустой проверкой.
        /// Один Update выполнился, когда команд не было.
        /// 2. Извлекаем все команды.
        /// 3. Обе команды должны быть извлечены.
        /// </summary>
        /// <param name="commands"></param>
        [Test()]
        public void Pop_TwoCommandsWithIdleUpdate_AllCommandsExtracted()
        {
            // ARRANGE
            var command1 = new Mock<ICommand>().Object;
            var command2 = new Mock<ICommand>().Object;

            var commandManager = new QueueCommandManager();



            // ACT

            commandManager.Push(command1);
            var factCommand1 = commandManager.Pop();
            

            // Холостой Update.
            var idleCommand = commandManager.Pop();
            

            // Вторая команда должна быть извлечена.
            commandManager.Push(command2);
            var factCommand2 = commandManager.Pop();
            


            // ASSERT
            factCommand1.Should().Be(command1);
            idleCommand.Should().BeNull();
            factCommand2.Should().Be(command2);
        }
    }
}