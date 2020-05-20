using JetBrains.Annotations;

using Zilon.Core.Graphs;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour
{
    /// <summary>
    /// Намерение на перемещние актёра.
    /// </summary>
    public class MoveIntention : IIntention
    {
        private readonly ISectorMap _map;

        public MoveIntention(IGraphNode targetNode, ISectorMap map)
        {
            TargetNode = targetNode;
            _map = map;
        }

        public IGraphNode TargetNode { get; }

        public IActorTask CreateActorTask([NotNull] IActor actor)
        {
            return CreateTaskInner(actor);
        }

        private MoveTask CreateTaskInner(IActor actor)
        {
            return new MoveTask(actor, TargetNode, _map, 500);
        }
    }
}
