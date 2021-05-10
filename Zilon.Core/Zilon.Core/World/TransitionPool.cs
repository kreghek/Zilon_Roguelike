using System.Collections.Concurrent;
using System.Linq;

using Zilon.Core.Persons;

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

        /// <inheritdoc />
        public bool CheckPersonInTransition(IPerson person)
        {
            var transition = _queue.ToArray().SingleOrDefault(x => x.Person == person);
            return transition != null;
        }
    }
}