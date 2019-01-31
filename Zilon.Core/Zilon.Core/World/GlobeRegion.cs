using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.World
{
    /// <summary>
    /// Граф провинции на глобальной карте.
    /// </summary>
    public class GlobeRegion : HexMap
    {
        /// <summary>
        /// Конструктор графа провинции.
        /// </summary>
        public GlobeRegion() : base(10)
        {
        }
    }
}
