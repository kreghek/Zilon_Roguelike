using System.Collections.Generic;

namespace Zilon.Core.MapGenerators.CellularAutomatonStyle
{
    /// <summary>
    /// Processing to change map created by generator.
    /// </summary>
    interface IRegionPostProcessor
    {
        IEnumerable<RegionDraft> Process(IEnumerable<RegionDraft> sourceRegions);
    }
}