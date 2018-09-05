using System;
using System.Threading.Tasks;

using Zilon.Core.Tactics.Behaviour.Bots;

namespace Zilon.Core.Tactics.Behaviour
{
    public class HumanActorTaskSource : IHumanActorTaskSource
    {
        private IActorTask _currentTask;
        private IIntention _currentIntesion;
        private readonly IDecisionSource _decisionSource;

        public HumanActorTaskSource(IDecisionSource decisionSource)
        {
            _decisionSource = decisionSource;
        }

        public void SwitchActor(IActor currentActor)
        {
            CurrentActor = currentActor;
        }

        public IActor CurrentActor { get; private set; }


        public void Intent(IIntention intention)
        {
            _currentIntesion = intention ?? throw new ArgumentException(nameof(intention));

            if (completionSource != null)
            {
                _currentTask = _currentIntesion.CreateActorTask(_currentTask, CurrentActor);

                if (_currentTask != null)
                {
                    completionSource.SetResult(new IActorTask[] { _currentTask });
                }
                else
                {
                    completionSource.SetResult(new IActorTask[0]);
                }
            }
        }

        public TaskCompletionSource<IActorTask[]> completionSource;

        public Task<IActorTask[]> GetActorTasksAsync(IActor actor)
        {
            if (CurrentActor == null)
            {
                throw new InvalidOperationException("Не выбран текущий ключевой актёр.");
            }

            if (actor != CurrentActor)
            {
                return Task.FromResult(new IActorTask[0]);
            }

            completionSource = new TaskCompletionSource<IActorTask[]>();

            var asyncTask = completionSource.Task;
            return asyncTask;
        }
    }
}