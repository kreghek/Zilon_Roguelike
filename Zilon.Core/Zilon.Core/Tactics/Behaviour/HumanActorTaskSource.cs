using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Zilon.Core.Tactics.Behaviour
{
    public class HumanActorTaskSource : IHumanActorTaskSource
    {
        private SpscChannel<IActorTask> _spscChannel;

        public HumanActorTaskSource()
        {
            _spscChannel = new SpscChannel<IActorTask>();
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

        public Task IntentAsync(IIntention intention)
        {
            if (_intentionWait)
            {
                throw new Exception();
            }

            var currentIntention = intention ?? throw new ArgumentNullException(nameof(intention));

            var actorTask = currentIntention.CreateActorTask(ActiveActor);

            _intentionWait = true;

            return _spscChannel.SendAsync(actorTask);
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

            return _spscChannel.ReceiveAsync();
        }

        public bool CanGetTask(IActor actor)
        {
            return actor == ActiveActor;
        }

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

    public interface ISender<in T>
    {
        Task SendAsync(T obj);
    }

    public interface IReceiver<T>
    {
        Task<T> ReceiveAsync();
    }

    public sealed class SpscChannel<T> : ISender<T>, IReceiver<T>, IDisposable
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        private readonly IProducerConsumerCollection<TaskCompletionSource<T>> _receivers =
            new ConcurrentQueue<TaskCompletionSource<T>>();

        private readonly IProducerConsumerCollection<T> _values = new ConcurrentQueue<T>();

        public async Task SendAsync(T obj)
        {
            await _semaphore.WaitAsync().ConfigureAwait(false);

            try
            {
                if (_receivers.TryTake(out var receiver))
                {
                    receiver.SetResult(obj);
                }
                else
                {
                    _values.TryAdd(obj);
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<T> ReceiveAsync()
        {
            TaskCompletionSource<T> source;
            await _semaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                if (_values.TryTake(out var value))
                {
                    return value;
                }
                else
                {
                    source = new TaskCompletionSource<T>();
                    _receivers.TryAdd(source);
                }
            }
            finally
            {
                _semaphore.Release();
            }

            return await source.Task.ConfigureAwait(false);
        }

        public void Dispose()
        {
            _semaphore.Dispose();
        }
    }

}