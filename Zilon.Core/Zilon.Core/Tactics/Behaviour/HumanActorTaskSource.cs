using System;
using System.Threading.Tasks;
using Zilon.Core.Common;
using Zilon.Core.PersonModules;

namespace Zilon.Core.Tactics.Behaviour
{
    public class HumanActorTaskSource<TContext> : IHumanActorTaskSource<TContext>
        where TContext : ISectorTaskSourceContext
    {
        private readonly IReceiver<IActorTask> _actorTaskReceiver;
        private readonly ISender<IActorTask> _actorTaskSender;
        private IActor _currentActorIntention;
        private bool _intentionWait;

        public HumanActorTaskSource()
        {
            var spscChannel = new SpscChannel<IActorTask>();
            _actorTaskSender = spscChannel;
            _actorTaskReceiver = spscChannel;
        }

        public async Task IntentAsync(IIntention intention, IActor activeActor)
        {
            if (_intentionWait)
            {
                // Это может происходить, если в клиентском коде предварительно не выполнили проверку CanIntent().
                // Проверка нужна, чтобы не допустить получение очереди задач.
                // Текущая реализация не допускает переопределение задач.
                // Поэтому каждое новое намерение будет складывать по новой задаче в очередь, пока выполняется текущая задача
                // Текущая задача выполняется в основном игровом цикле, который накручивает счётчик итераций, чтобы выполнить предусловия задачи.
                throw new InvalidOperationException(
                    "Попытка задать новое намерение, пока не выполнена текущая задача.");
            }

            var currentIntention = intention ?? throw new ArgumentNullException(nameof(intention));

            var actorTask = currentIntention.CreateActorTask(activeActor);

            _intentionWait = true;
            _currentActorIntention = activeActor;

            await _actorTaskSender.SendAsync(actorTask).ConfigureAwait(false);
        }

        public async Task<IActorTask> GetActorTaskAsync(IActor actor, TContext context)
        {
            // Тезисы:
            // Этот источник команд ждёт, пока игрок не укажет задачу.
            // Задача генерируется из намерения. Это значит, что ждать нужно, пока не будет задано намерение.

            return await _actorTaskReceiver.ReceiveAsync().ConfigureAwait(false);
        }

        //TODO Избавиться от синхронного варианта.
        // Сейчас он оставлен прото из-за тестов. Сложностей с удалением нет, кроме рутины.
        [Obsolete("Использовать асинк-вариант вместо этого")]
        public void Intent(IIntention intention, IActor activeActor)
        {
            IntentAsync(intention, activeActor).Wait();
        }

        public bool CanIntent()
        {
            return !_intentionWait && !CurrentActorSetAndIsDead();
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
            _currentActorIntention = null;
        }

        public void CancelTask(IActorTask cencelledActorTask)
        {
            _intentionWait = false;
            _currentActorIntention = null;
        }

        private bool CurrentActorSetAndIsDead()
        {
            return (_currentActorIntention?.Person?.GetModuleSafe<ISurvivalModule>()?.IsDead).GetValueOrDefault();
        }
    }
}