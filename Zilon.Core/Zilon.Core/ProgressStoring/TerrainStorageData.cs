using System;
using System.Linq;

using Zilon.Core.World;
using Zilon.Core.WorldGeneration;

namespace Zilon.Core.ProgressStoring
{
    public sealed class TerrainStorageData
    {
        public TerrainCell[][] Cells { get; private set; }

        /// <summary>
        /// Провинции мира.
        /// </summary>
        public GlobeRegionStorageData[] Regions { get; set; }

        public static TerrainStorageData Create(Terrain terrain)
        {
            if (terrain is null)
            {
                throw new ArgumentNullException(nameof(terrain));
            }

            var regionDict = terrain.Regions.ToDictionary(x => Guid.NewGuid(), x => x);
            var storageData = new TerrainStorageData
            {
                Cells = terrain.Cells,
                Regions = terrain.Regions.Select(x =>
                {
                    return GlobeRegionStorageData.Create(x);
                }).ToArray()
            };

            return storageData;
        }

        public Terrain Restore()
        {
            var terrain = new Terrain
            {
                Cells = Cells
            };

            return terrain;
        }
    }
}
