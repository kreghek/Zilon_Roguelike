using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Состояние конкретной работы перка.
    /// </summary>
    public class PerkJob: IJob
    {
        public JobSubScheme Scheme { get; }

        public int Progress { get; set; }
        public bool IsComplete { get; set; }

        public PerkJob(JobSubScheme scheme)
        {
            Scheme = scheme;
        }
    }
}
