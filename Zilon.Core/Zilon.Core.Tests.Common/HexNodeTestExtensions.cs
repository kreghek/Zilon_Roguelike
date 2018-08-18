using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tests.Common
{
    public static class HexNodeTestExtensions
    {
        public static HexNode SelectBy(this IEnumerable<HexNode> nodes, int offsetX, int offsetY)
        {
            return nodes.SingleOrDefault(n => n.OffsetX == offsetX && n.OffsetY == offsetY);
        }
    }
}
