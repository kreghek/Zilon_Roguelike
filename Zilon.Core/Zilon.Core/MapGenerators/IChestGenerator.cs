﻿using System.Collections.Generic;

using Zilon.Core.Schemes;
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
        /// <param name="map"> Карта сектора. Нужна для определения доступного места для сундука. </param>
        /// <param name="sectorSubScheme"> Схема сектора. По сути - настройки для размещения сундуков. </param>
        /// <param name="regions"> Регионы, в которых возможно размещение сундуков. </param>
        void CreateChests(IMap map, ISectorSubScheme sectorSubScheme, IEnumerable<MapRegion> regions);
    }
}