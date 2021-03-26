using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Zilon.Core.Commands
{
    public class QueueCommandManager : ICommandPool
    {
        private readonly ConcurrentQueue<ICommand> _queue;

        [ExcludeFromCodeCoverage]
        public QueueCommandManager()
        {
            _queue = new ConcurrentQueue<ICommand>();
        }

        public event EventHandler? CommandPushed;

        public ICommand? Pop()
        {
            if (_queue.Any())
            {
                if (_queue.TryDequeue(out var command))
                {
                    return command;
                }

                return null;
            }

            return null;
        }

        public void Push(ICommand command)
        {
            _queue.Enqueue(command);
            CommandPushed?.Invoke(this, EventArgs.Empty);
        }
    }
}