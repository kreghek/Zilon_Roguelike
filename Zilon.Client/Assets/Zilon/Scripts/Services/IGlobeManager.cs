using System.Collections.Generic;
using System.Threading.Tasks;

using Zilon.Core.World;
using Zilon.Core.WorldGeneration;

namespace Assets.Zilon.Scripts.Services
{
    public interface IGlobeManager
    {
        Globe CurrentGlobe { get; }

        Task GenerateGlobeAsync();

        Task<GlobeRegion> GenerateRegionAsync(TerrainCell cell);

        Dictionary<TerrainCell, GlobeRegion> Regions { get; }
    }
}