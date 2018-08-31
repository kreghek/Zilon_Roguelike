using System;
using System.Threading.Tasks;

using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour
{
    public class TmpTaskSource : IActorTaskSource
    {
        private readonly IDecisionSource _decisionSource;

        public TmpTaskSource(IDecisionSource decisionSource)
        {
            _decisionSource = decisionSource;
        }

        public IActorTask[] GetActorTasks(IMap map, IActorManager actorManager)
        {
            throw new NotImplementedException();
        }

        public Task<IActorTask[]> GetActorTasks(IActor actor)
        {
            return Task.FromResult(new IActorTask[] { new IdleTask(actor, _decisionSource) });
        }
    }
}
