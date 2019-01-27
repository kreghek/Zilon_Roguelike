using System.Collections.Generic;

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

        public void GenerateGlobe()
        {
            CurrentGlobe = _generator.GenerateGlobe();
        }

        public GlobeRegion GenerateRegion(TerrainCell cell)
        {
            return _generator.GenerateRegion(CurrentGlobe, cell);
        }
    }
}
