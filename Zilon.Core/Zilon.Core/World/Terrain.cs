using Zilon.Core.WorldGeneration;

namespace Zilon.Core.World
{
    /// <summary>
    /// Вся информация о территориях мира.
    /// </summary>
    public sealed class Terrain
    {
        /// <summary>
        /// Пров
        /// </summary>
        public TerrainCell[][] Cells { get; set; }

        public GlobeRegion[] Regions { get; set; }
    }
}
