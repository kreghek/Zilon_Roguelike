using System;
using System.Threading.Tasks;

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
        }

        private IActorTask currentTask;

        public Task<IActorTask[]> GetActorTasks(IActor actor)
        {
            if (currentTask == null || currentTask.IsComplete)
            {
                currentTask = new IdleTask(actor, _decisionSource);
            }

            return Task.FromResult(new IActorTask[] { currentTask });
        }
    }
}