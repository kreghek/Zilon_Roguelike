using System;
using System.Linq;
using Zilon.Core.Schemes;
using Zilon.Core.World;
using Zilon.Core.WorldGeneration;

namespace Zilon.Core.ProgressStoring
{
    public sealed class TerrainStorageData
    {
        public TerrainCell[][] Cells { get; set; }

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
                Cells = terrain.Cells,//.SelectMany(x=>x).ToArray(),
                Regions = terrain.Regions.Select(x =>
                {
                    return GlobeRegionStorageData.Create(x);
                }).ToArray()
            };

            return storageData;
        }

        public Terrain Restore(ISchemeService schemeService)
        {
            //var cells = new TerrainCell[40][];
            //for (var x = 0; x < 40; x++)
            //{
            //    for (var y = 0; y < 40; y++)
            //    {
            //        cells[x][y] = Cells[x * 40 + y];
            //    }
            //}

            var terrain = new Terrain
            {
                Cells = Cells,
                Regions = Regions.Select(x => x.Restore(schemeService)).ToArray()
            };

            return terrain;
        }
    }
}
