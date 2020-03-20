using System.Linq;

using Zilon.Core.Players;
using Zilon.Core.World;
using Zilon.Core.WorldGeneration;

namespace Zilon.Core.ProgressStoring
{
    public sealed class HumanPlayerStorageData
    {
        public string SectorSid { get; set; }

        public int CurrentGlobeNodeX { get; set; }
        public int CurrentGlobeNodeY { get; set; }

        public int TerrainX { get; set; }
        public int TerrainY { get; set; }

        public static HumanPlayerStorageData Create(HumanPlayer humanPlayer)
        {
            var storageData = new HumanPlayerStorageData();
            storageData.CurrentGlobeNodeX = humanPlayer.GlobeNode.OffsetX;
            storageData.CurrentGlobeNodeY = humanPlayer.GlobeNode.OffsetY;
            storageData.SectorSid = humanPlayer.SectorSid;
            storageData.TerrainX = humanPlayer.Terrain.Coords.X;
            storageData.TerrainY = humanPlayer.Terrain.Coords.Y;
            return storageData;
        }

        public void Restore(HumanPlayer humanPlayer, Globe globe, IWorldManager worldManager)
        {
            var terrainCoords = new OffsetCoords(TerrainX, TerrainY);
            var terrainCell = globe.Terrain.SelectMany(x => x).Single(x => x.Coords == terrainCoords);
            humanPlayer.Terrain = terrainCell;
            humanPlayer.SectorSid = SectorSid;

            var globeRegion = worldManager.Regions[terrainCell];
            humanPlayer.GlobeNode = globeRegion.RegionNodes.Single(x => x.OffsetX == CurrentGlobeNodeX && x.OffsetY == CurrentGlobeNodeY);
        }
    }
}
