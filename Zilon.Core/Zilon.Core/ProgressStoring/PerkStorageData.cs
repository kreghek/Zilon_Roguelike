namespace Zilon.Core.ProgressStoring
{
    public sealed class PerkStorageData
    {
        public PerkJobStorageData[] Jobs { get; set; }

        public int? Level { get; set; }

        public string Sid { get; set; }

        public int? SubLevel { get; set; }
    }
}