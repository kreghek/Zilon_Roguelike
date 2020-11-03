using Zilon.Core.Schemes;

namespace Zilon.Core.ProgressStoring
{
    public sealed class PerkJobStorageData
    {
        public JobType Type { get; set; }
        public JobScope Scope { get; set; }
        public int Progress { get; set; }
        public bool IsComplete { get; set; }
    }
}