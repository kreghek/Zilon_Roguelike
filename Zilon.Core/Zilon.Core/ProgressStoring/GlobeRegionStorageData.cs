using System.Linq;

using Zilon.Core.World;
using Zilon.Core.WorldGeneration;

namespace Zilon.Core.ProgressStoring
{
    public sealed class GlobeRegionStorageData
    {
        public string Id { get; set; }
        public GlobeRegionNodeStorageData[] Nodes { get; set; }

        public static GlobeRegionStorageData Create(GlobeRegion globeRegion, TerrainCell terrainCell)
        {
            var storageData = new GlobeRegionStorageData();

            storageData.Id = terrainCell.Coords.ToString();
            storageData.Nodes = globeRegion.RegionNodes.Select(x => GlobeRegionNodeStorageData.Create(x)).ToArray();

            return storageData;
        }
    }
}
