using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    ///     Состояние конкретной работы перка.
    /// </summary>
    public class PerkJob : IJob
    {
        [ExcludeFromCodeCoverage]
        public PerkJob(IJobSubScheme scheme)
        {
            Scheme = scheme;
        }

        /// <inheritdoc />
        public IJobSubScheme Scheme { get; }

        /// <inheritdoc />
        public int Progress { get; set; }

        /// <inheritdoc />
        public bool IsComplete { get; set; }
    }
}