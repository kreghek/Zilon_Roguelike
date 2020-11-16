using System;

using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Tactics
{
    internal class TaskState
    {
        private readonly int _valueToExecute;

        public TaskState(IActor actor, IActorTask task, IActorTaskSource<ISectorTaskSourceContext> taskSource)
        {
            Actor = actor ?? throw new ArgumentNullException(nameof(actor));
            Task = task ?? throw new ArgumentNullException(nameof(task));
            TaskSource = taskSource ?? throw new ArgumentNullException(nameof(taskSource));

            Counter = Task.Cost;
            _valueToExecute = Task.Cost / 2;
        }

        public IActor Actor { get; }

        public int Counter { get; private set; }

        public IActorTask Task { get; }

        public bool TaskComplete => Counter <= 0;

        public bool TaskIsExecuting => Counter == _valueToExecute;

        public IActorTaskSource<ISectorTaskSourceContext> TaskSource { get; }

        public void UpdateCounter()
        {
            Counter--;
        }
    }
}