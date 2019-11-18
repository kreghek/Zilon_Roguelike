using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Tactics.Spatial;
using Zilon.Core.WorldGeneration;

namespace Zilon.Core.World
{
    /// <summary>
    /// Граф провинции на глобальной карте.
    /// </summary>
    public class GlobeRegion : HexMap
    {
        public TerrainCell TerrainCell { get; set; }

        /// <summary>
        /// Конструктор графа провинции.
        /// </summary>
        public GlobeRegion(int segmentSize) : base(segmentSize)
        {
        }

        /// <summary>
        /// Вспомогательное свойство региона для того, чтобы каждый раз не приводить узлы к ожидаемому типу.
        /// </summary>
        public IEnumerable<GlobeRegionNode> RegionNodes
        {
            get
            {
                return Nodes.OfType<GlobeRegionNode>();
            }
        }
    }
}
