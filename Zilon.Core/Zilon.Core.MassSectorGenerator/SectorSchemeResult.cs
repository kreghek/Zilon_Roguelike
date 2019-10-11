using System;

using Zilon.Core.Schemes;

namespace Zilon.Core.MassSectorGenerator
{
    public sealed class SectorSchemeResult
    {
        public SectorSchemeResult(ILocationScheme location, ISectorSubScheme sector)
        {
            Location = location ?? throw new ArgumentNullException(nameof(location));
            Sector = sector ?? throw new ArgumentNullException(nameof(sector));
        }

        public ILocationScheme Location { get; }

        public ISectorSubScheme Sector { get; }
    }
}
