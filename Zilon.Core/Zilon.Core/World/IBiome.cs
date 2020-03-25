using System.Collections.Generic;
using Zilon.Core.Graphs;
using Zilon.Core.Schemes;

namespace Zilon.Core.World
{
    public interface IBiome: IGraph
    {
        ILocationScheme LocationScheme { get; }
        IEnumerable<SectorNode> Sectors { get; }
    }
}