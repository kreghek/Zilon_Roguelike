using System;

using Zilon.Core.Schemes;
using Zilon.Core.Tactics;

namespace Zilon.Core.World
{
    public sealed class SectorNode : ISectorNode
    {
        public SectorNode(IBiome biom, ISectorSubScheme sectorScheme)
        {
            Biome = biom ?? throw new ArgumentNullException(nameof(biom));
            SectorScheme = sectorScheme ?? throw new ArgumentNullException(nameof(sectorScheme));
            State = SectorNodeState.SchemeKnown;
        }

        public SectorNode()
        {
        }

        public ISector Sector { get; private set; }

        public IBiome Biome { get; private set; }

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

        public void BindSchemeInfo(IBiome biom, ISectorSubScheme sectorScheme)
        {
            if (State != SectorNodeState.SchemeUnknown)
            {
                throw new InvalidOperationException("Неверное состояние узла.");
            }

            Biome = biom ?? throw new ArgumentNullException(nameof(biom));
            SectorScheme = sectorScheme ?? throw new ArgumentNullException(nameof(sectorScheme));
            State = SectorNodeState.SchemeKnown;
        }

        public SectorNodeState State { get; private set; }

        public override string ToString()
        {
            return $"[{Biome.LocationScheme}] {SectorScheme}";
        }
    }
}