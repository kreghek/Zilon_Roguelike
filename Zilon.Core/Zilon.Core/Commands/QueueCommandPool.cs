using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Zilon.Core.Commands
{
    public class QueueCommandPool : ICommandPool
    {
        private readonly ConcurrentQueue<ICommand> _queue;

        [ExcludeFromCodeCoverage]
        public QueueCommandPool()
        {
            _queue = new ConcurrentQueue<ICommand>();
        }

        public bool IsEmpty => !_queue.Any();

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