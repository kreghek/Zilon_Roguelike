namespace Zilon.Core.ProgressStoring
{
    public sealed class PropStorageData
    {
        public int Count { get; set; }

        public int Durable { get; set; }

        public string Sid { get; set; }

        public PropType Type { get; set; }
    }
}