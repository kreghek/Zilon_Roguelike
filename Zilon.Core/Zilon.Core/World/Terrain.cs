using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.World
{
    /// <summary>
    /// Вся информация о территориях мира.
    /// </summary>
    public sealed class Terrain: HexMap
    {
        public Terrain(int segmentSize) : base(segmentSize)
        {
        }

        /// <summary>
        /// Вспомогательное свойство региона для того, чтобы каждый раз не приводить узлы к ожидаемому типу.
        /// </summary>
        public IEnumerable<TerrainNode> TerrainNodes
        {
            get
            {
                return Nodes.OfType<TerrainNode>();
            }
        }
    }
}
