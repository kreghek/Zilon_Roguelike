using System;
using System.Threading.Tasks;

using Zilon.Core.Common;

namespace Zilon.Core.Tactics.Behaviour
{
    public class HumanActorTaskSource<TContext> : IHumanActorTaskSource<TContext> where TContext: ISectorTaskSourceContext
    {
        private readonly ISender<IActorTask> _actorTaskSender;
        private readonly IReceiver<IActorTask> _actorTaskReceiver;

        public HumanActorTaskSource()
        {
            var spscChannel = new SpscChannel<IActorTask>();
            _actorTaskSender = spscChannel;
            _actorTaskReceiver = spscChannel;
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

        private bool _intentionWait;

        public async Task IntentAsync(IIntention intention)
        {
            if (_intentionWait)
            {
                // Это может происходить, если в клиентском коде предварительно не выполнили проверку CanIntent().
                // Проверка нужна, чтобы не допустить получение очереди задач.
                // Текущая реализация не допускает переопределение задач.
                // Поэтому каждое новое намерение будет складывать по новой задаче в очередь, пока выполняется текущая задача
                // Текущая задача выполняется в основном игровом цикле, который накручивает счётчик итераций, чтобы выполнить предусловия задачи.
                throw new InvalidOperationException("Попытка задать новое намерение, пока не выполнена текущая задача.");
            }

            var currentIntention = intention ?? throw new ArgumentNullException(nameof(intention));

            var actorTask = currentIntention.CreateActorTask(ActiveActor);

            _intentionWait = true;

            await _actorTaskSender.SendAsync(actorTask).ConfigureAwait(false);
        }

        public Task<IActorTask> GetActorTaskAsync(IActor actor, TContext context)
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

            return _actorTaskReceiver.ReceiveAsync();
        }

        public bool CanGetTask(IActor actor)
        {
            return actor == ActiveActor;
        }

        //TODO Избавиться от синхронного варианта.
        // Сейчас он оставлен прото из-за тестов. Сложностей с удалением нет, кроме рутины.
        [Obsolete("Использовать асинк-вариант вместо этого")]
        public void Intent(IIntention intention)
        {
            IntentAsync(intention).Wait();
        }

        public bool CanIntent()
        {
            return !_intentionWait;
        }

        public void ProcessTaskExecuted(IActorTask actorTask)
        {
            // Пока ничего не делаем.
            // Это задел для парного оружия.
            // Парное оружие будет позволять выполнять удар вторым оружием,
            // когда задача выполнена, но не закончена.
        }

        public void ProcessTaskComplete(IActorTask actorTask)
        {
            _intentionWait = false;
        }
    }
}