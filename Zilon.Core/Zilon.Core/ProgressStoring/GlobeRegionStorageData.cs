using System.Linq;

using Zilon.Core.Schemes;
using Zilon.Core.World;

namespace Zilon.Core.ProgressStoring
{
    public sealed class GlobeRegionStorageData
    {
        //public string Id { get; set; }
        //public GlobeRegionNodeStorageData[] Nodes { get; set; }

        //public OffsetCoords TerrainCoords { get; set; }

        //public static GlobeRegionStorageData Create(Province globeRegion)
        //{
        //    if (globeRegion is null)
        //    {
        //        throw new System.ArgumentNullException(nameof(globeRegion));
        //    }

        //    var storageData = new GlobeRegionStorageData();

        //    storageData.Id = globeRegion.GlobeCoords.Coords.ToString();
        //    storageData.Nodes = globeRegion.ProvinceNodes.Select(x => GlobeRegionNodeStorageData.Create(x)).ToArray();
        //    storageData.TerrainCoords = globeRegion.GlobeCoords.Coords;

        //    return storageData;
        //}

        //public Province Restore(ISchemeService schemeService, World.TerrainCell[][] cells)
        //{
        //    if (schemeService is null)
        //    {
        //        throw new System.ArgumentNullException(nameof(schemeService));
        //    }

        //    var globeNode = new Province(20);
        //    globeNode.GlobeCoords = cells.SelectMany(x => x).Single(x => x.Coords == TerrainCoords);

        //    foreach (var storedNode in Nodes)
        //    {
        //        var scheme = schemeService.GetScheme<ILocationScheme>(storedNode.SchemeSid);
        //        var node = new ProvinceNode(storedNode.Coords.X, storedNode.Coords.Y, scheme)
        //        {
        //            IsBorder = storedNode.IsBorder,
        //            IsHome = storedNode.IsHome,
        //            IsStart = storedNode.IsStart,
        //            IsTown = storedNode.IsTown,
        //            ObservedState = storedNode.Observed
        //        };
        //        globeNode.AddNode(node);
        //    }

        //    return globeNode;
        //}
    }
}
