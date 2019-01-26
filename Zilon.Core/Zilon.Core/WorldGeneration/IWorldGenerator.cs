using Zilon.Core.World;

namespace Zilon.Core.WorldGeneration
{
    public interface IWorldGenerator
    {
        Globe GenerateGlobe();

        GlobeRegion GenerateRegion(Globe globe, TerrainCell cell);
    }
}