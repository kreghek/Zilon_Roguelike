using System;

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
            _currentIntesion = intention ?? throw new ArgumentNullException(nameof(intention));
        }

        public IActorTask[] GetActorTasks(IActor actor)
        {
            if (actor != CurrentActor)
            {
                return new IActorTask[0];
            }

            var currentTaskIsComplete = _currentTask?.IsComplete;
            if (currentTaskIsComplete != null && !currentTaskIsComplete.Value && _currentIntesion != null)
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
    }
}