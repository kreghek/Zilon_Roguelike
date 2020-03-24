using System;

using Zilon.Core.Graphs;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;

namespace Zilon.Core.World
{
    public sealed class SectorNode : IGraphNode
    {
        public SectorNode(ISectorSubScheme sectorScheme)
        {
            SectorScheme = sectorScheme ?? throw new ArgumentNullException(nameof(sectorScheme));
        }

        public ISector Sector { get; private set; }

        public ISectorSubScheme SectorScheme { get; }

        public void MaterializeSector(ISector sector)
        {
            Sector = sector ?? throw new ArgumentNullException(nameof(sector));
            IsSectorMaterialized = true;
        }

        public bool IsSectorMaterialized { get; private set; }
    }
}
