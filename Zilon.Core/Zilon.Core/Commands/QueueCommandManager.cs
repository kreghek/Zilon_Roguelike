using System.Collections.Generic;

namespace Zilon.Core.Commands
{
    public class QueueCommandManager : ICommandManager
    {
        private readonly Queue<ICommand> _queue;

        public QueueCommandManager()
        {
            _queue = new Queue<ICommand>();
        }

        public ICommand Pop()
        {
            if (_queue.Count == 0)
                return null;

            return _queue.Dequeue();
        }

        public void Push(ICommand command)
        {
            _queue.Enqueue(command);
        }
    }
}
