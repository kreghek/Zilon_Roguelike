using System.Collections.Generic;

using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators
{
    public interface ICitizenGenerator
    {
        void CreateCitizens(ISector sector,
            IBotPlayer botPlayer,
            IEnumerable<MapRegion> citizenRegions);
    }
}
