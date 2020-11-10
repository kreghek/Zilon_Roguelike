using System;
using System.Collections.Generic;
using System.Text;

namespace Zilon.Core.MapGenerators.CellularAutomatonStyle
{
    class Size7MapPostProcessor : IRegionPostProcessor
    {
        private readonly IMapRuleManager _mapRuleManager;

        public Size7MapPostProcessor(IMapRuleManager mapRuleManager)
        {
            _mapRuleManager = mapRuleManager ?? throw new ArgumentNullException(nameof(mapRuleManager));
        }

        public IEnumerable<RegionDraft> Process(IEnumerable<RegionDraft> sourceRegions)
        {

        }
    }
}
