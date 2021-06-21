using System;

namespace Zilon.Core.Tactics.Behaviour
{
    public class SectorTransitTask : ActorTaskBase
    {
        public SectorTransitTask(IActor actor, IActorTaskContext context) : base(actor, context)
        {
        }

        public override void Execute()
        {
            var actorNode = Actor.Node;
            var transition = TransitionDetection.Detect(Context.Sector.Map.Transitions, new[] { actorNode });
            if (transition != null)
            {
                Actor.MoveToOtherSector(Context.Sector, transition);
            }
            else
            {
                throw new InvalidOperationException("Попытка выполнить переход из узла, где нет перехода.");
            }
        }
    }
}