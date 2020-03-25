using Zilon.Core.Graphs;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;

namespace Zilon.Core.World
{
    public interface ISectorNode: IGraphNode
    {
        Biome Biome { get; }
        ISector Sector { get; }
        ISectorSubScheme SectorScheme { get; set; }
        SectorNodeState State { get; }

        void BindSchemeInfo(Biome biom, ISectorSubScheme sectorScheme);
        void MaterializeSector(ISector sector);
    }
}