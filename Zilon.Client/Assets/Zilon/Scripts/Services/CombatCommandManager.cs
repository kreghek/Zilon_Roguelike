using System.Collections.Generic;
using Assets.Zilon.Scripts.Models.Commands;

namespace Assets.Zilon.Scripts.Services
{
    class CombatCommandManager : ICommandManager
    {
        private readonly Queue<ICommand<ICommandContext>> queue;

        public CombatCommandManager()
        {
            queue = new Queue<ICommand<ICommandContext>>();
        }

        public ICommand<ICommandContext> Pop()
        {
            if (queue.Count == 0)
                return null;

            return queue.Dequeue();
        }

        public void Push(ICommand<ICommandContext> command)
        {
            queue.Enqueue(command);
        }
    }
}
