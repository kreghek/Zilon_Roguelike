using System.Collections.Generic;
using System.Linq;

namespace Zilon.Core.World
{
    public static class GlobeHelper
    {
        public static ProvinceNode GetCenterLocationNode(IEnumerable<ProvinceNode> locationNodeViewModels)
        {
            var xGroupedNodes = locationNodeViewModels
                .GroupBy(node => node.OffsetX)
                .OrderBy(nodeGroup => nodeGroup.Key);

            var xGroupCount = xGroupedNodes.Count();

            var xGroupCenterOffset = xGroupCount / 2;

            var xGroupCenter = xGroupedNodes.Skip(xGroupCenterOffset).First();

            var yCount = xGroupCenter.Count();

            var yOffset = yCount / 2;

            var centerNode = xGroupCenter.Skip(yOffset).First();

            return centerNode;
        }
    }
}
