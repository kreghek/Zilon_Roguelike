using Zilon.Core.Commands;

namespace Zilon.Core.Tests.Commands
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class QueueCommandManagerTests
    {
        /// <summary>
        ///     1. В системе есть набор команд, размещённых в менеджере за одну итерацию.
        ///     2. Извлекаем все команды.
        ///     3. Команды должны быть такими, какими их поместили в порядке очереди.
        /// </summary>
        [Test]
        public void Pop_GetOneCommand_AllCommandsExtracted()
        {
            // ARRANGE
            ICommand[] commands = GetOneCommand();

            QueueCommandManager commandManager = new QueueCommandManager();
            foreach (ICommand command in commands)
            {
                commandManager.Push(command);
            }

            // ACT
            AssertPopCommands(commands, commandManager);
        }

        /// <summary>
        ///     1. В системе есть набор команд, размещённых в менеджере за одну итерацию.
        ///     2. Извлекаем все команды.
        ///     3. Команды должны быть такими, какими их поместили в порядке очереди.
        /// </summary>
        [Test]
        public void Pop_FakeCommands2_AllCommandsExtracted()
        {
            // ARRANGE
            ICommand[] commands = GetTwoCommands();

            QueueCommandManager commandManager = new QueueCommandManager();
            foreach (ICommand command in commands)
            {
                commandManager.Push(command);
            }

            // ACT
            AssertPopCommands(commands, commandManager);
        }

        private static void AssertPopCommands(ICommand[] commands, QueueCommandManager commandManager)
        {
            foreach (ICommand expectedCommand in commands)
            {
                ICommand factCommand = commandManager.Pop();

                // ASSERT
                factCommand.Should().Be(expectedCommand);
            }
        }

        /// <summary>
        ///     1. В системе выполнили две команды с промежуточной пустой проверкой.
        ///     Один Update выполнился, когда команд не было.
        ///     2. Извлекаем все команды.
        ///     3. Обе команды должны быть извлечены.
        /// </summary>
        [Test]
        public void Pop_TwoCommandsWithIdleUpdate_AllCommandsExtracted()
        {
            // ARRANGE
            var command1 = new Mock<ICommand>().Object;
            var command2 = new Mock<ICommand>().Object;

            QueueCommandManager commandManager = new QueueCommandManager();

            // ACT

            commandManager.Push(command1);
            ICommand factCommand1 = commandManager.Pop();

            // Холостой Update.
            ICommand idleCommand = commandManager.Pop();

            // Вторая команда должна быть извлечена.
            commandManager.Push(command2);
            ICommand factCommand2 = commandManager.Pop();

            // ASSERT
            factCommand1.Should().Be(command1);
            idleCommand.Should().BeNull();
            factCommand2.Should().Be(command2);
        }

        private static ICommand[] GetOneCommand()
        {
            return new[] {CreateFakeCommand()};
        }

        private static ICommand[] GetTwoCommands()
        {
            return new[] {CreateFakeCommand(), CreateFakeCommand()};
        }

        private static ICommand CreateFakeCommand()
        {
            return new Mock<ICommand>().Object;
        }
    }
}