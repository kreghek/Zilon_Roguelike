namespace Zilon.Core.ProgressStoring
{
    public sealed class LocalityStorageData
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public OffsetCoords Coords { get; set; }

        public string RealmId { get; set; }

        public int Population { get; set; }

        public LocalityBranchStorageData[] Branches { get; set; }
    }
}
