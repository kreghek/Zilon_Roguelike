using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Zilon.Core.Common
{
    public sealed class SpscChannel<T> : ISender<T>, IReceiver<T>, IDisposable
    {
        private readonly IProducerConsumerCollection<TaskCompletionSource<T>> _receivers;
        private readonly SemaphoreSlim _semaphore;
        private readonly IProducerConsumerCollection<T> _values;

        public SpscChannel()
        {
            _semaphore = new SemaphoreSlim(1);
            _receivers = new ConcurrentQueue<TaskCompletionSource<T>>();
            _values = new ConcurrentQueue<T>();
        }

        public void Dispose()
        {
            _semaphore.Dispose();
        }

        public void CancelReceiving()
        {
            var receiverListCopy = _receivers.ToArray();
            foreach (var receiver in receiverListCopy)
            {
                // Call the receiver to stop.
                receiver.SetCanceled();

                // Remove the receiver from inner list beacuse it will not await anymore.
                _receivers.TryTake(out var _);
            }
        }

        public async Task<T> ReceiveAsync(CancellationToken cancellationToken)
        {
            TaskCompletionSource<T> source;
            await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
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

        public async Task SendAsync(T obj, CancellationToken cancellationToken)
        {
            await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                if (_receivers.TryTake(out var receiver))
                {
                    if (!receiver.TrySetResult(obj))
                    {
                        Debug.Fail("Error in concurrency.");
                    }
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
    }
}