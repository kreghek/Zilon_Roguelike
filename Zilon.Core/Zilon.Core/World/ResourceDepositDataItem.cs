namespace Zilon.Core.World
{
    public sealed class ResourceDepositDataItem
    {
        public ResourceDepositDataItem(SectorResourceType resourceType, float share)
        {
            ResourceType = resourceType;
            Share = share;
        }

        public SectorResourceType ResourceType { get; }

        public float Share { get; }
    }
}
