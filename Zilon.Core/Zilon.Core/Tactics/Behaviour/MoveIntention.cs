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
        private readonly IMap _map;

        public MoveIntention(IGraphNode targetNode, IMap map)
        {
            TargetNode = targetNode;
            _map = map;
        }

        public IGraphNode TargetNode { get; }

        public IActorTask CreateActorTask([CanBeNull] IActorTask currentTask, [NotNull] IActor actor)
        {
            var currentMoveTask = currentTask as MoveTask;
            if (currentMoveTask == null)
            {
                return CreateTaskInner(actor);
            }

            if (currentMoveTask.TargetNode == TargetNode)
            {
                return currentMoveTask;
            }

            return CreateTaskInner(actor);
        }

        private MoveTask CreateTaskInner(IActor actor)
        {
            return new MoveTask(actor, TargetNode, _map);
        }
    }
}
