namespace Zilon.Core.ProgressStoring
{
    public sealed class PerkStorageData
    {
        public string Sid { get; set; }

        public int? Level { get; set; }

        public int? SubLevel { get; set; }

        public PerkJobStorageData[] Jobs { get; set; }
    }
}
