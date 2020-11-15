using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.World;

namespace Zilon.Core.Tests.Common
{
    public class TestMaterializedSectorNode : ISectorNode
    {
        public TestMaterializedSectorNode(ISectorSubScheme sectorScheme)
        {
            SectorScheme = sectorScheme ?? throw new ArgumentNullException(nameof(sectorScheme));
        }

        public IBiome Biome { get; }

        public ISector Sector { get; }

        public ISectorSubScheme SectorScheme { get; }

        public SectorNodeState State => SectorNodeState.SectorMaterialized;

        public void BindSchemeInfo(IBiome biom, ISectorSubScheme sectorScheme)
        {
            throw new NotImplementedException();
        }

        public void MaterializeSector(ISector sector)
        {
            throw new NotImplementedException();
        }
    }
}