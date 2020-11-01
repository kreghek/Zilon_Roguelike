namespace Zilon.Core.World
{
    public interface IResourceDepositData
    {
        IEnumerable<ResourceDepositDataItem> Items { get; }
    }
}