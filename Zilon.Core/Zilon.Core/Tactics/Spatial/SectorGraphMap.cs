using System.Collections.Generic;

using Zilon.Core.MapGenerators.RoomStyle;

namespace Zilon.Core.Tactics.Spatial
{
    public sealed class SectorGraphMap : GraphMap, ISectorMap
    {
        public Dictionary<IMapNode, RoomTransition> Transitions { get; }

        public SectorGraphMap()
        {
            Transitions = new Dictionary<IMapNode, RoomTransition>();
        }
    }
}
