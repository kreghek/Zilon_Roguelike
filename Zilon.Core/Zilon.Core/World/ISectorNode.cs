using Zilon.Core.Graphs;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;

namespace Zilon.Core.World
{
    public interface ISectorNode: IGraphNode
    {
        IBiome Biome { get; }
        
        ISector Sector { get; }
        
        ISectorSubScheme SectorScheme { get; }
        
        SectorNodeState State { get; }

        void BindSchemeInfo(IBiome biom, ISectorSubScheme sectorScheme);

        void MaterializeSector(ISector sector);
    }
}