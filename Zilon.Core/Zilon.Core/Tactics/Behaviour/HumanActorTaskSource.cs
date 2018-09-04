using System;
using System.Threading.Tasks;
using Zilon.Core.Players;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;

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

        public IActorTask[] GetActorTasks(IMap map, IActorManager actorManager)
        {
            var currentTaskIsComplete = _currentTask?.IsComplete;
            if (currentTaskIsComplete != null && !currentTaskIsComplete.Value)
            {
                return new IActorTask[] { _currentTask };
            }

            if (CurrentActor == null)
            {
                throw new InvalidOperationException("Не выбран текущий ключевой актёр.");
            }

            if (_currentIntesion == null)
            {
                return new IActorTask[0];
            }

            _currentTask = _currentIntesion.CreateActorTask(_currentTask, CurrentActor);
            _currentIntesion = null;

            if (_currentTask != null)
            {
                return new IActorTask[] { _currentTask };
            }
            else
            {
                return new IActorTask[0];
            }
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

        private IActorTask currentTask;

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