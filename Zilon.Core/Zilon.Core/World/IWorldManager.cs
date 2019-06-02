using System.Collections.Generic;

using Zilon.Core.WorldGeneration;

namespace Zilon.Core.World
{
    /// <summary>
    /// Сервис, управляющий глобальной картой.
    /// </summary>
    public interface IWorldManager
    {
        /// <summary>
        /// Глобальная карта.
        /// </summary>
        Globe Globe { get; set; }

        /// <summary>
        /// Текущие сгенерированые провинции относительно ячеек глобальной карты.
        /// </summary>
        Dictionary<TerrainCell, GlobeRegion> Regions { get; }

        /// <summary>
        /// Обновление состояния узлов провинции.
        /// </summary>
        /// <param name="region">Провинция, которая обновляется.</param>
        void UpdateRegionNodes(GlobeRegion region);
    }
}
