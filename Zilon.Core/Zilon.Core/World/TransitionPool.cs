using System.Collections.Concurrent;

namespace Zilon.Core.World
{
    /// <summary>
    /// Base implementation of the transition pool.
    /// </summary>
    public class TransitionPool : ITransitionPool
    {
        private readonly ConcurrentQueue<TransitionPoolItem> _queue;

        public TransitionPool()
        {
            _queue = new ConcurrentQueue<TransitionPoolItem>();
        }

        /// <inheritdoc />
        public void Push(TransitionPoolItem poolItem)
        {
            _queue.Enqueue(poolItem);
        }

        /// <inheritdoc />
        public TransitionPoolItem? Pop()
        {
            if (!_queue.TryDequeue(out var item))
            {
                return null;
            }

            return item;
        }
    }
}