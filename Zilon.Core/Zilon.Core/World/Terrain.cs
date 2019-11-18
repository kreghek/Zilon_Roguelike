using Zilon.Core.WorldGeneration;

namespace Zilon.Core.World
{
    /// <summary>
    /// Вся информация о территориях мира.
    /// </summary>
    public sealed class Terrain
    {
        /// <summary>
        /// Сетка всех узлов уровня провинций.
        /// </summary>
        public TerrainCell[][] Cells { get; set; }

        /// <summary>
        /// Провинции с уих узлами.
        /// </summary>
        public GlobeRegion[] Regions { get; set; }
    }
}
