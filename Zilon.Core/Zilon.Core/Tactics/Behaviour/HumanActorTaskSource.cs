using System;
using System.Threading.Tasks;

namespace Zilon.Core.Tactics.Behaviour
{
    public class HumanActorTaskSource : IHumanActorTaskSource
    {
        private TaskCompletionSource<IActorTask> _taskCompletionSource;

        private IActorTask _currentTask;

        public void SwitchActor(IActor currentActor)
        {
            CurrentActor = currentActor;
        }

        public IActor CurrentActor { get; private set; }


        public void Intent(IIntention intention)
        {
            _taskCompletionSource = new TaskCompletionSource<IActorTask>();

            var currentIntention = intention ?? throw new ArgumentNullException(nameof(intention));
            var actorTask = currentIntention.CreateActorTask(_currentTask, CurrentActor);
            _currentTask = actorTask;
            _taskCompletionSource.SetResult(_currentTask);
        }

        public Task<IActorTask> GetActorTaskAsync(IActor actor)
        {
            // Тезисы:
            // Этот источник команд ждёт, пока игрок не укажет задачу.
            // Задача генерируется из намерения. Это значит, что ждать нужно, пока не будет задано намерение.

            if (CurrentActor is null)
            {
                throw new InvalidOperationException("Не выбран текущий ключевой актёр.");
            }

            if (actor != CurrentActor)
            {
                throw new InvalidOperationException($"Получение задачи актёра без предварительно проверки в {nameof(CanGetTask)}");
            }

            // Намерение может не задаваться каждую итерацию. Потому что задачи длятся больше одной итерации.
            // Поэтому если уже назначена задача и она еще не выполнена,
            // то предполагаем, что игрок не изменял текущую задачу.
            var currentTaskIsComplete = _currentTask?.IsComplete;
            if (currentTaskIsComplete != null && !currentTaskIsComplete.Value)
            {
                return Task.FromResult(_currentTask);
            }

            return _taskCompletionSource.Task;
        }

        public bool CanGetTask(IActor actor)
        {
            return actor == CurrentActor;
        }
    }
}