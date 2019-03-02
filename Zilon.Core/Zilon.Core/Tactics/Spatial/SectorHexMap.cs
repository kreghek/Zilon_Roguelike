using System.Collections.Generic;

using Zilon.Core.MapGenerators.RoomStyle;

namespace Zilon.Core.Tactics.Spatial
{
    public class SectorHexMap : HexMap, ISectorMap
    {
        private const int SegmentSize = 200;

        public SectorHexMap() : this(SegmentSize)
        {
        }

        public SectorHexMap(int segmentSize) : base(segmentSize)
        {
            Transitions = new Dictionary<IMapNode, RoomTransition>();
        }

        public Dictionary<IMapNode, RoomTransition> Transitions { get; }
    }
}
