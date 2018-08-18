using System;

using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour
{
    public class HumanActorTaskSource : IHumanActorTaskSource
    {
        private IActorTask _currentTask;
        private IIntention _currentIntesion;

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
            //TODO Чтобы отменять текущие намерения, нужно указывать CancelIntention.
            // Становление null для намерения происходит только изнутри, после создания задачи.
            if (intention == null)
            {
                throw new ArgumentException(nameof(intention));
            }

            _currentIntesion = intention;
        }
    }
}