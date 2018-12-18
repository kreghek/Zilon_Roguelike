using System.Collections.Generic;

using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators
{
    public interface IChestGenerator
    {
        void CreateChests(IMap map, IEnumerable<MapRegion> regions);
    }
}