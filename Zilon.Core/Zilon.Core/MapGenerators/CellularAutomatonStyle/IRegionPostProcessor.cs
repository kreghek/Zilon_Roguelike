using System.Collections.Generic;

namespace Zilon.Core.MapGenerators.CellularAutomatonStyle
{
    interface IRegionPostProcessor
    {
        IEnumerable<RegionDraft> Process(IEnumerable<RegionDraft> sourceRegions);
    }
}