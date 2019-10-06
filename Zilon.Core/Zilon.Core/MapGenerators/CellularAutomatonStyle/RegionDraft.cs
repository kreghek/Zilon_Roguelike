using System;

namespace Zilon.Core.MapGenerators.CellularAutomatonStyle
{
    /// <summary>
    /// Промежуточная стуктура для разметы регионов.
    /// </summary>
    internal sealed class RegionDraft
    {
        public RegionDraft(OffsetCoords[] coords)
        {
            Coords = coords ?? throw new ArgumentNullException(nameof(coords));
        }

        public OffsetCoords[] Coords { get; }
    }
}
