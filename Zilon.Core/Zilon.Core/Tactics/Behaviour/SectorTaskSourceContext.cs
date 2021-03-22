using System;
using System.Threading;

namespace Zilon.Core.Tactics.Behaviour
{
    public sealed class SectorTaskSourceContext : ISectorTaskSourceContext
    {
        public SectorTaskSourceContext(ISector sector, CancellationToken cancellationToken)
        {
            Sector = sector ?? throw new ArgumentNullException(nameof(sector));
            CancellationToken = cancellationToken;
        }

        public ISector Sector { get; }
        public CancellationToken? CancellationToken { get; }
    }
}