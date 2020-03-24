using System;

using Zilon.Core.Graphs;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;

namespace Zilon.Core.World
{
    public sealed class SectorNode : IGraphNode
    {
        public SectorNode(Biom biom, ISectorSubScheme sectorScheme)
        {
            Biom = biom ?? throw new ArgumentNullException(nameof(biom));
            SectorScheme = sectorScheme ?? throw new ArgumentNullException(nameof(sectorScheme));
            State = SectorNodeState.SchemeKnown;
        }

        public SectorNode()
        {
        }

        public ISector Sector { get; private set; }

        public Biom Biom { get; private set; }

        public ISectorSubScheme SectorScheme { get; set; }

        public void MaterializeSector(ISector sector)
        {
            Sector = sector ?? throw new ArgumentNullException(nameof(sector));
            State = SectorNodeState.SectormMaterialized;
        }

        public void BindSchemeInfo(Biom biom, ISectorSubScheme sectorScheme)
        {
            Biom = biom ?? throw new ArgumentNullException(nameof(biom));
            SectorScheme = sectorScheme ?? throw new ArgumentNullException(nameof(sectorScheme));
            State = SectorNodeState.SchemeKnown;
        }

        public SectorNodeState State { get; private set; }
    }

    public enum SectorNodeState
    {
        SchemeUnknown,
        SchemeKnown,
        SectormMaterialized
    }
}
