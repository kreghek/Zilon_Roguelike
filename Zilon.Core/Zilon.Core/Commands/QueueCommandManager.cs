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

        public ICommand Pop()
        {
            if (!_queue.Any())
            {
                return null;
            }

            return _queue.Dequeue();
        }

        public void Push(ICommand command)
        {
            _queue.Enqueue(command);
        }
    }
}
