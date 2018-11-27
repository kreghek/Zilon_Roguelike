using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <inheritdoc />
    /// <summary>
    /// Состояние конкретной работы перка.
    /// </summary>
    public class PerkJob : IJob
    {
        public IJobSubScheme Scheme { get; }

        public int Progress { get; set; }

        public bool IsComplete { get; set; }

        [ExcludeFromCodeCoverage]
        public PerkJob(IJobSubScheme scheme)
        {
            Scheme = scheme;
        }
    }
}
