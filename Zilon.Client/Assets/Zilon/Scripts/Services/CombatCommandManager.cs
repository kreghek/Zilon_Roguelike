using System.Collections.Generic;
using Assets.Zilon.Scripts.Models.Commands;

namespace Assets.Zilon.Scripts.Services
{
    class CombatCommandManager : ICommandManager
    {
        private readonly Queue<ICommand> queue;

        public CombatCommandManager()
        {
            queue = new Queue<ICommand>();
        }

        public ICommand Pop()
        {
            if (queue.Count == 0)
                return null;

            return queue.Dequeue();
        }

        public void Push(ICommand command)
        {
            queue.Enqueue(command);
        }
    }
}
