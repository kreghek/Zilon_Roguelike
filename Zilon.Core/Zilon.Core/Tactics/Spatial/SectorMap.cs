using System.Collections.Generic;

using Zilon.Core.MapGenerators.RoomStyle;

namespace Zilon.Core.Tactics.Spatial
{
    public class SectorMap : HexMap, ISectorMap
    {
        private const int SegmentSize = 200;

        public SectorMap() : this(SegmentSize)
        {
        }

        public SectorMap(int segmentSize) : base(segmentSize)
        {
            Transitions = new Dictionary<IMapNode, RoomTransition>();
        }

        public Dictionary<IMapNode, RoomTransition> Transitions { get; }
    }
}
