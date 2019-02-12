using System.Collections.Generic;

using Zilon.Core.MapGenerators.RoomStyle;

namespace Zilon.Core.Tactics.Spatial
{
    public class SectorMap : HexMap, ISectorMap
    {
        private const int SegmentSize = 200;

        public SectorMap(): base(SegmentSize)
        {
            Transitions = new Dictionary<IMapNode, RoomTransition>();
        }

        public Dictionary<IMapNode, RoomTransition> Transitions { get; }
    }
}
