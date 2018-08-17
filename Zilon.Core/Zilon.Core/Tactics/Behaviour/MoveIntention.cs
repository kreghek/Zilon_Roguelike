using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour
{
    /// <summary>
    /// Намерение на перемещние актёра.
    /// </summary>
    public class MoveIntention : IIntention
    {
        private readonly IMap _map;
        private readonly IMapNode _targetNode;

        public MoveIntention(IMapNode targetNode, IMap map)
        {
            _targetNode = targetNode;
            _map = map;
        }

        public IActorTask CreateActorTask(IActorTask currentTask, IActor actor)
        {
            var currentMoveTask = currentTask as MoveTask;
            if (currentMoveTask == null)
            {
                return CreateTaskInner(actor);
            }

            if (currentMoveTask.TargetNode == _targetNode)
            {
                return currentMoveTask;
            }

            return CreateTaskInner(actor);
        }

        private MoveTask CreateTaskInner(IActor actor)
        {
            return new MoveTask(actor, _targetNode, _map);
        }
    }
}
