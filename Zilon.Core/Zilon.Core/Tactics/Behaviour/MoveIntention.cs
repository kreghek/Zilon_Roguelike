using System;

using Zilon.Core.Graphs;
using Zilon.Core.PersonModules;

namespace Zilon.Core.Tactics.Behaviour
{
    /// <summary>
    /// Намерение на перемещние актёра.
    /// </summary>
    public class MoveIntention : IIntention
    {
        private readonly ISector _sector;

        public MoveIntention(IGraphNode targetNode, ISector sector)
        {
            TargetNode = targetNode ?? throw new ArgumentNullException(nameof(targetNode));
            _sector = sector ?? throw new ArgumentNullException(nameof(sector));
        }

        public IGraphNode TargetNode { get; }

        private MoveTask CreateMoveTaskInner(IActor actor)
        {
            var taskContext = new ActorTaskContext(_sector);

            var movingModule = actor.Person.GetModuleSafe<IMovingModule>();
            if (movingModule is null)
            {
                return new MoveTask(actor, taskContext, TargetNode, taskContext.Sector.Map);
            }

            var moveCost = movingModule.CalculateCost();
            return new MoveTask(actor, taskContext, TargetNode, taskContext.Sector.Map, moveCost);
        }

        public IActorTask CreateActorTask(IActor actor)
        {
            return CreateMoveTaskInner(actor);
        }
    }
}