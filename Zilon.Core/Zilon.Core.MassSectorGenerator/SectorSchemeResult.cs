using System;
using Zilon.Core.Schemes;

namespace Zilon.Core.MassSectorGenerator
{
    /// <summary>
    /// Результат выбора сектора. Используется масс-генератором при выборе случайной локации и сектора.
    /// </summary>
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