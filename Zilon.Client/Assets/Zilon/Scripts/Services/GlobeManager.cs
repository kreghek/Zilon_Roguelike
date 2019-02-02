using System.Collections.Generic;
using System.Threading.Tasks;
using Zilon.Core.World;
using Zilon.Core.WorldGeneration;

namespace Assets.Zilon.Scripts.Services
{
    public class GlobeManager : IGlobeManager
    {
        private readonly IWorldGenerator _generator;

        public GlobeManager(IWorldGenerator generator)
        {
            _generator = generator;
            Regions = new Dictionary<TerrainCell, GlobeRegion>();
        }

        public Globe CurrentGlobe { get; private set; }
        public Dictionary<TerrainCell, GlobeRegion> Regions { get; }

        public async Task GenerateGlobeAsync()
        {
            CurrentGlobe = await _generator.GenerateGlobeAsync();
        }

        public async Task<GlobeRegion> GenerateRegionAsync(TerrainCell cell)
        {
            return await _generator.GenerateRegionAsync(CurrentGlobe, cell);
        }
    }
}
