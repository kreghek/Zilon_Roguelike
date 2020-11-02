using JetBrains.Annotations;

using Zilon.Core.Graphs;
using Zilon.Core.PersonModules;
using Zilon.Core.World;

namespace Zilon.Core.Tactics.Behaviour
{
    /// <summary>
    /// Намерение на перемещние актёра.
    /// </summary>
    public class MoveIntention : IIntention
    {
        private readonly ISector _sector;
        private readonly IGlobe _globe;

        public MoveIntention(IGraphNode targetNode, ISector sector, IGlobe globe)
        {
            TargetNode = targetNode ?? throw new System.ArgumentNullException(nameof(targetNode));
            _sector = sector ?? throw new System.ArgumentNullException(nameof(sector));
            _globe = globe ?? throw new System.ArgumentNullException(nameof(globe));
        }

        public IGraphNode TargetNode { get; }

        public IActorTask CreateActorTask([NotNull] IActor actor)
        {
            return CreateTaskInner(actor);
        }

        private MoveTask CreateTaskInner(IActor actor)
        {
            var taskContext = new ActorTaskContext(_sector, _globe);

            return CreateMoveTask(actor, taskContext);
        }

        private MoveTask CreateMoveTask(IActor actor, ActorTaskContext taskContext)
        {
            var movingModule = actor.Person.GetModuleSafe<IMovingModule>();
            if (movingModule is null)
            {
                return new MoveTask(actor, taskContext, TargetNode, taskContext.Sector.Map);
            }
            else
            {
                var moveCost = movingModule.CalculateCost();
                return new MoveTask(actor, taskContext, TargetNode, taskContext.Sector.Map, moveCost);
            }
        }
    }
}