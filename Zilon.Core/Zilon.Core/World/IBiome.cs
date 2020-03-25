using System.Collections.Generic;
using Zilon.Core.Schemes;

namespace Zilon.Core.World
{
    public interface IBiome
    {
        ILocationScheme LocationScheme { get; }
        IEnumerable<SectorNode> Sectors { get; }
    }
}