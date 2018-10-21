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

        public IActorTask[] GetActorTasks(IActor actor)
        {
            if (actor != CurrentActor)
            {
                return new IActorTask[0];
            }

            var currentTaskIsComplete = _currentTask?.IsComplete;
            if (currentTaskIsComplete != null && !currentTaskIsComplete.Value && _currentIntention == null)
            {
                return new[] { _currentTask };
            }

            if (CurrentActor == null)
            {
                throw new InvalidOperationException("Не выбран текущий ключевой актёр.");
            }

            if (_currentIntention == null)
            {
                return new IActorTask[0];
            }

            _currentTask = _currentIntention.CreateActorTask(_currentTask, CurrentActor);
            _currentIntention = null;

            if (_currentTask != null)
            {
                return new[] { _currentTask };
            }

            return new IActorTask[0];
        }
    }
}