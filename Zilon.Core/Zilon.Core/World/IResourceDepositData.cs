using System.Collections.Generic;

namespace Zilon.Core.World
{
    public interface IResourceDepositData
    {
        IEnumerable<ResourceDepositDataItem> Items { get; }
    }

    public sealed class ResourceDepositData : IResourceDepositData
    {
        public ResourceDepositData(IEnumerable<ResourceDepositDataItem> items)
        {
            Items = items ?? throw new System.ArgumentNullException(nameof(items));
        }

        public IEnumerable<ResourceDepositDataItem> Items { get; }
    }

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
