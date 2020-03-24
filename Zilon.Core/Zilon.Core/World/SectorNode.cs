using System;

using Zilon.Core.Graphs;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;

namespace Zilon.Core.World
{
    public sealed class SectorNode : IGraphNode
    {
        public SectorNode(Biome biom, ISectorSubScheme sectorScheme)
        {
            Biome = biom ?? throw new ArgumentNullException(nameof(biom));
            SectorScheme = sectorScheme ?? throw new ArgumentNullException(nameof(sectorScheme));
            State = SectorNodeState.SchemeKnown;
        }

        public SectorNode()
        {
        }

        public ISector Sector { get; private set; }

        public Biome Biome { get; private set; }

        public ISectorSubScheme SectorScheme { get; set; }

        public void MaterializeSector(ISector sector)
        {
            if (State != SectorNodeState.SchemeKnown)
            {
                throw new InvalidOperationException("Неверное состояние узла.");
            }

            Sector = sector ?? throw new ArgumentNullException(nameof(sector));
            State = SectorNodeState.SectorMaterialized;
        }

        public void BindSchemeInfo(Biome biom, ISectorSubScheme sectorScheme)
        {
            if (State != SectorNodeState.SchemeUnknown)
            {
                throw new InvalidOperationException("Неверное состояние узла.");
            }

            Biome = biom ?? throw new ArgumentNullException(nameof(biom));
            SectorScheme = sectorScheme ?? throw new ArgumentNullException(nameof(sectorScheme));
            State = SectorNodeState.SchemeKnown;
        }

        public override string ToString()
        {
            return $"[{Biome.LocationScheme}] {SectorScheme}";
        }

        public SectorNodeState State { get; private set; }
    }
}
