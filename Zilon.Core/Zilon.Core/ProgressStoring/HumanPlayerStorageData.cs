using Zilon.Core.Players;
using Zilon.Core.World;

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
            //storageData.CurrentGlobeNodeX = humanPlayer.GlobeNode.OffsetX;
            //storageData.CurrentGlobeNodeY = humanPlayer.GlobeNode.OffsetY;
            storageData.SectorSid = humanPlayer.SectorSid;
            //storageData.TerrainX = humanPlayer.Terrain.Coords.X;
            //storageData.TerrainY = humanPlayer.Terrain.Coords.Y;
            return storageData;
        }

        public void Restore(HumanPlayer humanPlayer, Globe globe, IGlobeManager worldManager)
        {
            if (worldManager is null)
            {
                throw new System.ArgumentNullException(nameof(worldManager));
            }

            var terrainCoords = new OffsetCoords(TerrainX, TerrainY);
            //var terrainCell = globe.Terrain.Cells.SelectMany(x => x).Single(x => x.Coords == terrainCoords);
            //humanPlayer.Terrain = terrainCell;
            humanPlayer.SectorSid = SectorSid;

            //var globeRegion = worldManager.Globe.Terrain.Regions.Single(x=>x.GlobeCoords == terrainCell);
            //humanPlayer.GlobeNode = globeRegion.ProvinceNodes.Single(x => x.OffsetX == CurrentGlobeNodeX && x.OffsetY == CurrentGlobeNodeY);
        }
    }
}
