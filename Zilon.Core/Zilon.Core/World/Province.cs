using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.World
{
    /// <summary>
    /// Граф провинции на глобальной карте.
    /// </summary>
    public class Province : HexMap
    {
        public OffsetCoords GlobeCoords { get; set; }

        /// <summary>
        /// Конструктор графа провинции.
        /// </summary>
        public Province(int segmentSize) : base(segmentSize)
        {
        }

        /// <summary>
        /// Вспомогательное свойство региона для того, чтобы каждый раз не приводить узлы к ожидаемому типу.
        /// </summary>
        public IEnumerable<ProvinceNode> ProvinceNodes
        {
            get
            {
                return Nodes.OfType<ProvinceNode>();
            }
        }
    }
}
