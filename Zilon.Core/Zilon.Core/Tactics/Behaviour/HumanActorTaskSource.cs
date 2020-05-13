using System;
using System.Threading.Tasks;

namespace Zilon.Core.Tactics.Behaviour
{
    public class HumanActorTaskSource : IHumanActorTaskSource
    {
        private TaskCompletionSource<IActorTask> _taskCompletionSource;

        private IActorTask _currentTask;
        private IIntention _currentIntention;

        public void SwitchActor(IActor currentActor)
        {
            CurrentActor = currentActor;
        }

        public IActor CurrentActor { get; private set; }


        public void Intent(IIntention intention)
        {
            var currentIntention = intention ?? throw new ArgumentNullException(nameof(intention));
            var actorTask = currentIntention.CreateActorTask(_currentTask, CurrentActor);
            _currentTask = actorTask;
            _taskCompletionSource.SetResult(_currentTask);
        }

        public Task<IActorTask> GetActorTaskAsync(IActor actor)
        {
            if (actor != CurrentActor)
            {
                throw new InvalidOperationException();
            }

            var currentTaskIsComplete = _currentTask?.IsComplete;
            if (currentTaskIsComplete != null && !currentTaskIsComplete.Value && _currentIntention == null)
            {
                return Task.FromResult(_currentTask);
            }

            if (CurrentActor == null)
            {
                throw new InvalidOperationException("Не выбран текущий ключевой актёр.");
            }

            if (_currentIntention == null)
            {
                throw new InvalidOperationException();
            }

            _currentTask = _currentIntention.CreateActorTask(_currentTask, CurrentActor);
            _currentIntention = null;

            if (_currentTask != null)
            {
                Task.FromResult(_currentTask);
            }

            throw new InvalidOperationException();
        }

        public bool CanGetTask(IActor actor)
        {
            return actor == CurrentActor;
        }
    }
}