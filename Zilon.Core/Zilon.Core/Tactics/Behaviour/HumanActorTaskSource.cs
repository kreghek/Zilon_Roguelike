using System;
using System.Threading.Tasks;

namespace Zilon.Core.Tactics.Behaviour
{
    public class HumanActorTaskSource : IHumanActorTaskSource
    {
        private TaskCompletionSource<IActorTask> _taskCompletionSource;

        public HumanActorTaskSource()
        {
            _taskCompletionSource = new TaskCompletionSource<IActorTask>();
        }

        public HumanActorTaskSource(IActor activeActor) : this()
        {
            SwitchActiveActor(activeActor);
        }

        public void SwitchActiveActor(IActor currentActor)
        {
            ActiveActor = currentActor;
        }

        public IActor ActiveActor { get; private set; }


        public void Intent(IIntention intention)
        {
            var currentIntention = intention ?? throw new ArgumentNullException(nameof(intention));

            var actorTask = currentIntention.CreateActorTask(ActiveActor);
            _taskCompletionSource.SetResult(actorTask);
            
            if (_taskRequested)
            {
                RefreshTaskCompetitonSource();
            }
        }

        public Task<IActorTask> GetActorTaskAsync(IActor actor)
        {
            // Тезисы:
            // Этот источник команд ждёт, пока игрок не укажет задачу.
            // Задача генерируется из намерения. Это значит, что ждать нужно, пока не будет задано намерение.

            if (ActiveActor is null)
            {
                throw new InvalidOperationException("Не выбран текущий ключевой актёр.");
            }

            if (actor != ActiveActor)
            {
                throw new InvalidOperationException($"Получение задачи актёра без предварительно проверки в {nameof(CanGetTask)}");
            }

            if (_taskCompletionSource is null)
            {
                throw new InvalidOperationException("Сначала нужно задать намерение.");
            }

            var actorTaskTask = _taskCompletionSource.Task;
            _taskRequested = true;

            return actorTaskTask;
        }

        private bool _taskRequested = false;

        private void RefreshTaskCompetitonSource()
        {
            _taskCompletionSource = new TaskCompletionSource<IActorTask>();
        }

        public bool CanGetTask(IActor actor)
        {
            return actor == ActiveActor;
        }
    }
}