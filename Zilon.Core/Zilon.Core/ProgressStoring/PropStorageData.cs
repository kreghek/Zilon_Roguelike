namespace Zilon.Core.ProgressStoring
{
    public sealed class PropStorageData
    {
        public string Sid { get; set; }

        public PropType Type { get; set; }

        public int Durable { get; set; }

        public int Count { get; set; }
    }
}
