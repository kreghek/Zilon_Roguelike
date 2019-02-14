using System;
using Zilon.Core.MapGenerators.RoomStyle;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    public sealed class SectorExitEventArgs: EventArgs
    {
        public SectorExitEventArgs(MapRegion mapRegion, RoomTransition transition)
        {
            MapRegion = mapRegion ?? throw new ArgumentNullException(nameof(mapRegion));
            Transition = transition ?? throw new ArgumentNullException(nameof(transition));
        }

        public MapRegion MapRegion { get; }

        public RoomTransition Transition { get; }
    }
}
