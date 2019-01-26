using Zilon.Core.World;
using Zilon.Core.WorldGeneration;

namespace Assets.Zilon.Scripts.Services
{
    public interface IGlobeManager
    {
        Globe CurrentGlobe { get; }

        void GenerateGlobe();

        GlobeRegion GenerateRegion(TerrainCell cell);
    }
}