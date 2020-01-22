using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Zilon.Core.Commands
{
    public class QueueCommandManager<TContext> : ICommandManager<TContext>
    {
        private readonly Queue<ICommand<TContext>> _queue;

        [ExcludeFromCodeCoverage]
        public QueueCommandManager()
        {
            _queue = new Queue<ICommand<TContext>>();
        }

        public event EventHandler CommandPushed;

        public ICommand<TContext> Pop()
        {
            if (_queue.Any())
            {
                return _queue.Dequeue();
            }
            
            return null;
        }

        public void Push(ICommand<TContext> command)
        {
            _queue.Enqueue(command);
            CommandPushed?.Invoke(this, EventArgs.Empty);
        }
    }
}
