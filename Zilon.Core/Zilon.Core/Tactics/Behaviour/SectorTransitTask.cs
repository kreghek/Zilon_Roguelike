using System;
using JetBrains.Annotations;

namespace Zilon.Core.Tactics.Behaviour
{
    internal class SectorTransitTask : ActorTaskBase
    {
        public SectorTransitTask([NotNull] IActor actor, IActorTaskContext context) : base(actor, context)
        {
        }

        public override int Cost => 1;

        public override void Execute()
        {
            var actorNode = Actor.Node;
            var transition = TransitionDetection.Detect(Context.Sector.Map.Transitions, new[] {actorNode});
            if (transition != null)
            {
                Context.Sector.UseTransition(Actor, transition);
            }
            else
            {
                throw new InvalidOperationException("Попытка выполнить переход из узла, где нет перехода.");
            }
        }
    }
}