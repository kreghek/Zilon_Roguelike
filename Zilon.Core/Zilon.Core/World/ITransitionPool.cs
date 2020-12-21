using System.Collections.Concurrent;

using Zilon.Core.Graphs;
using Zilon.Core.Tactics;

namespace Zilon.Core.World
{
    public interface ITransitionPool
    {
        void Push(TransitionPoolItem poolItem);

        TransitionPoolItem Pop();
    }

    public class TransitionPool : ITransitionPool
    {
        private readonly ConcurrentQueue<TransitionPoolItem> _queue;

        public TransitionPool()
        {
            _queue = new ConcurrentQueue<TransitionPoolItem>();
        }

        public void Push(TransitionPoolItem poolItem)
        {
            _queue.Enqueue(poolItem);
        }

        public TransitionPoolItem Pop()
        {
            if (!_queue.TryDequeue(out var item))
            {
                return null;
            }

            return item;
        }
    }

    public class TransitionPoolItem
    {
        public TransitionPoolItem(IActor actor, ISector nextSector, ISector oldSector, IGraphNode oldNode)
        {
            Actor = actor;
            NextSector = nextSector;
            OldSector = oldSector;
            OldNode = oldNode;
        }

        public IActor Actor { get; }
        public ISector NextSector { get; }
        public ISector OldSector { get; }
        public IGraphNode OldNode { get; }
    }
}