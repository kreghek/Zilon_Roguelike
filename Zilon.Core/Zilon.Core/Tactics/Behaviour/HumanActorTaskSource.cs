using System;

namespace Zilon.Core.Tactics.Behaviour
{
    public class HumanActorTaskSource : IHumanActorTaskSource
    {
        private IActorTask _currentTask;
        private IIntention _currentIntention;

        public void SwitchActor(IActor currentActor)
        {
            CurrentActor = currentActor;
        }

        public IActor CurrentActor { get; private set; }


        public void Intent(IIntention intention)
        {
            _currentIntention = intention ?? throw new ArgumentNullException(nameof(intention));
        }

        public IActorTask GetActorTask(IActor actor)
        {
            if (actor != CurrentActor)
            {
                throw new InvalidOperationException();
            }

            var currentTaskIsComplete = _currentTask?.IsComplete;
            if (currentTaskIsComplete != null && !currentTaskIsComplete.Value && _currentIntention == null)
            {
                return _currentTask;
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
                return _currentTask;
            }

            throw new InvalidOperationException();
        }

        public bool CanGetTask(IActor actor)
        {
            return actor == CurrentActor;
        }
    }
}