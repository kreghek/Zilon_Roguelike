namespace Zilon.Core.World
{
    public sealed class ResourceMaterializationMap : IResourceMaterializationMap
    {
        public IResourceDepositData GetDepositData(ISectorNode sectorNode)
        {
            var items = new[] { 
                new ResourceDepositDataItem(SectorResourceType.Iron, 10),
                new ResourceDepositDataItem(SectorResourceType.Stones, 10),
                new ResourceDepositDataItem(SectorResourceType.WaterPuddles, 10),
                new ResourceDepositDataItem(SectorResourceType.CherryBrushes, 10),
            };
            return new ResourceDepositData(items);
        }
    }
}
