using System;

using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.World
{
    public class SectorInfo
    {
        public SectorInfo(ISector sector,
                          GlobeRegion region,
                          GlobeRegionNode regionNode)
        {
            Sector = sector ?? throw new ArgumentNullException(nameof(sector));
            Region = region ?? throw new ArgumentNullException(nameof(region));
            RegionNode = regionNode ?? throw new ArgumentNullException(nameof(regionNode));
        }

        public ISector Sector { get; }
        public GlobeRegion Region { get; }
        public GlobeRegionNode RegionNode { get; }
    }
}
