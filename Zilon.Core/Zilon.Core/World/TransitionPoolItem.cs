using Zilon.Core.Graphs;
using Zilon.Core.Persons;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.World
{
    /// <summary>
    /// Item represent data to a person can transit to other sector level.
    /// </summary>
    public class TransitionPoolItem
    {
        public TransitionPoolItem(IPerson person, IActorTaskSource<ISectorTaskSourceContext> actorTaskSource, ISector nextSector, ISector oldSector, IGraphNode oldNode)
        {
            Person = person ?? throw new System.ArgumentNullException(nameof(person));
            TaskSource = actorTaskSource ?? throw new System.ArgumentNullException(nameof(actorTaskSource));
            NextSector = nextSector ?? throw new System.ArgumentNullException(nameof(nextSector));
            OldSector = oldSector ?? throw new System.ArgumentNullException(nameof(oldSector));
            OldNode = oldNode ?? throw new System.ArgumentNullException(nameof(oldNode));
        }

        public IPerson Person { get; }
        public IActorTaskSource<ISectorTaskSourceContext> TaskSource { get; }
        public ISector NextSector { get; }
        public IGraphNode OldNode { get; }
        public ISector OldSector { get; }
    }
}