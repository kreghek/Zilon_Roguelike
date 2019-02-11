using System.Collections.Generic;

using Zilon.Core.WorldGeneration;

namespace Zilon.Core.World
{
    public sealed class WorldManager : IWorldManager
    {
        public WorldManager()
        {
            Regions = new Dictionary<TerrainCell, GlobeRegion>();
        }

        public Globe Globe { get; set; }
        public Dictionary<TerrainCell, GlobeRegion> Regions { get; }
    }
}
