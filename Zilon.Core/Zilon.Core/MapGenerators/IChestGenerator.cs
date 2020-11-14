using System.Collections.Generic;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators
{
    /// <summary>
    /// Генератор сундуков в секторе.
    /// </summary>
    /// <remarks>
    /// Главная задача генератора сундуков - это распределение сундуков по сектору.
    /// </remarks>
    public interface IChestGenerator
    {
        /// <summary>
        /// Создать сундуки в секторе.
        /// </summary>
        /// <param name="sector"> Сектор, для которого происходит генерация. </param>
        /// <param name="sectorSubScheme">Схема сектора. По сути - настройки для размещения сундуков.</param>
        /// <param name="regions"> Регионы, в которых возможно размещение сундуков. </param>
        void CreateChests(ISector sector, ISectorSubScheme sectorSubScheme, IEnumerable<MapRegion> regions);
    }
}