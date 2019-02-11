using System;

using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    public sealed class SectorExitEventArgs: EventArgs
    {
        public SectorExitEventArgs(MapRegion mapRegion)
        {
            MapRegion = mapRegion ?? throw new ArgumentNullException(nameof(mapRegion));
        }

        public MapRegion MapRegion { get; }
    }
}
