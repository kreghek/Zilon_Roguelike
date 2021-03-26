using NUnit.Framework;
using Zilon.Core.Client.Sector;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Zilon.Core.Commands;
using System.Threading;
using System.Threading.Tasks;

namespace Zilon.Core.Client.Sector.Tests
{
    [TestFixture()]
    [Parallelizable(ParallelScope.All)]
    public class CommandLoopUpdaterTests
    {
        [Test()]
        [Timeout(1000)]
        public void StartAsync_CommandInPool_ExecutesCommand()
        {
            // ARRANGE

            var contextMock = new Mock<ICommandLoopContext>();
            contextMock.SetupGet(x => x.HasNextIteration).Returns(true);
            contextMock.Setup(x => x.WaitForUpdate(CancellationToken.None)).Returns(Task.CompletedTask);
            var context = contextMock.Object;

            var commandMock = new Mock<ICommand>();
            var command = commandMock.Object;

            var commandPool = new TestCommandPool();

            var commandLoopUpdater = new CommandLoopUpdater(context, commandPool);

            // ACT

            commandLoopUpdater.StartAsync(CancellationToken.None).ConfigureAwait(false);

            commandPool.Push(command);

            // ASSERT

            commandMock.Verify(x=>x.Execute(), Times.Once);
        }

        private sealed class TestCommandPool : ICommandPool
        {
            public event EventHandler CommandPushed;

            private ICommand _storedCommand;

            public ICommand Pop()
            {
                var commandToPop = _storedCommand;
                _storedCommand = null;
                return commandToPop;
            }

            public void Push(ICommand command)
            {
                _storedCommand = command;
            }
        }
    }
}