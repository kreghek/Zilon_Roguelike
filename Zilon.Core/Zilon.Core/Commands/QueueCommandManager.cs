using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Zilon.Core.Commands
{
    public class QueueCommandManager : ICommandManager
    {
        private readonly Queue<ICommand> _queue;

        [ExcludeFromCodeCoverage]
        public QueueCommandManager()
        {
            _queue = new Queue<ICommand>();
        }

        public event EventHandler CommandPushed;

        public ICommand Pop()
        {
            if (_queue.Any())
            {
                return _queue.Dequeue();
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