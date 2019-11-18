using Zilon.Core.WorldGeneration;

namespace Zilon.Core.ProgressStoring
{
    public sealed class TerrainStorageData
    {
        public TerrainCell[][] Terrain { get; private set; }

        public static TerrainStorageData Create(TerrainCell[][] terrain)
        {
            var storageData = new TerrainStorageData
            {
                Terrain = terrain
            };

            return storageData;
        }

        public TerrainCell[][] Restore()
        {
            return Terrain;
        }
    }
}
