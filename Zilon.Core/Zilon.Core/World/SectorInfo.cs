using System;

using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.World
{
    public class SectorInfo
    {
        public SectorInfo(IActorManager actorManager,
                          IPropContainerManager propContainerManager,
                          ISector sector,
                          GlobeRegion region,
                          GlobeRegionNode regionNode,
                          IActorTaskSource actorTaskSource)
        {
            ActorManager = actorManager ?? throw new ArgumentNullException(nameof(actorManager));
            PropContainerManager = propContainerManager ?? throw new ArgumentNullException(nameof(propContainerManager));
            Sector = sector ?? throw new ArgumentNullException(nameof(sector));
            Region = region ?? throw new ArgumentNullException(nameof(region));
            RegionNode = regionNode ?? throw new ArgumentNullException(nameof(regionNode));
            ActorTaskSource = actorTaskSource ?? throw new ArgumentNullException(nameof(actorTaskSource));
        }

        public IActorManager ActorManager { get; }
        public IPropContainerManager PropContainerManager { get; }
        public ISector Sector { get; }
        public GlobeRegion Region { get; }
        public GlobeRegionNode RegionNode { get; }
        public IActorTaskSource ActorTaskSource { get; }
    }
}
