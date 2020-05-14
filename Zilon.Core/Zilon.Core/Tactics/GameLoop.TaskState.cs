using System;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Tactics
{
    public sealed partial class GameLoop
    {
        private class TaskState
        {
            private readonly int _valueToExecute;

            public TaskState(IActor actor, IActorTask task)
            {
                Actor = actor ?? throw new ArgumentNullException(nameof(actor));
                Task = task ?? throw new ArgumentNullException(nameof(task));

                Counter = Task.Cost;
                _valueToExecute = Task.Cost / 2;
            }

            public IActorTask Task { get; }
            public int Counter { get; private set; }
            public void UpdateCounter()
            {
                Counter--;
            }

            public bool TaskIsExecuting { get => Counter == _valueToExecute; }

            public bool TaskComplete { get => Counter <= 0; }

            public IActor Actor { get; }
        }
    }
}
