using System.Collections.Generic;
using Assets.Zilon.Scripts.Commands;

namespace Assets.Zilon.Scripts.Services
{
    class CombatCommandManager : ICommandManager<CombatCommandBase>
    {
        private readonly Queue<CombatCommandBase> queue;

        public CombatCommandManager()
        {
            queue = new Queue<CombatCommandBase>();
        }

        public CombatCommandBase Pop()
        {
            return queue.Dequeue();
        }

        public void Push(CombatCommandBase command)
        {
            queue.Enqueue(command);
        }
    }
}
