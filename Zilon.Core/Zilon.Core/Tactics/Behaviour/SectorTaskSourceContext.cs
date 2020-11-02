using System;

using Zilon.Core.World;

namespace Zilon.Core.Tactics.Behaviour
{
    public sealed class SectorTaskSourceContext : ISectorTaskSourceContext
    {
        public SectorTaskSourceContext(ISector sector, IGlobe globe)
        {
            Sector = sector ?? throw new ArgumentNullException(nameof(sector));
            Globe = globe ?? throw new ArgumentNullException(nameof(globe));
        }

        public ISector Sector { get; }
        public IGlobe Globe { get; }
    }
}