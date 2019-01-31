using Zilon.Core.Schemes;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.World
{
    /// <summary>
    /// Узел локации провинции в графе провинции.
    /// </summary>
    public class GlobeRegionNode : HexNode
    {
        /// <summary>
        /// Конструктор узла провинции.
        /// </summary>
        /// <param name="x"> Координата X в сетке гексов. </param>
        /// <param name="y"> Координата Y в сетке гексов. </param>
        /// <param name="scheme"> Схема провинции. </param>
        public GlobeRegionNode(int x, int y, ILocationScheme scheme) : base(x, y)
        {
            Scheme = scheme;
        }

        /// <summary>
        /// Схема провинции.
        /// </summary>
        public ILocationScheme Scheme { get; }
    }
}
