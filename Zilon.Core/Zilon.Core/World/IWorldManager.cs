using System.Collections.Generic;

using Zilon.Core.WorldGeneration;

namespace Zilon.Core.World
{
    public interface IWorldManager
    {
        Globe Globe { get; set; }

        Dictionary<TerrainCell, GlobeRegion> Regions { get; }
    }
}
