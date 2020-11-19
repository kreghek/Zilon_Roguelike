using System;
using System.Collections.Generic;

namespace Zilon.Core.MapGenerators.CellularAutomatonStyle
{
    /// <summary>
    /// Черновик региона карты.
    /// Промежуточная структура, которая используется для заливки
    /// кластеров в фабрике карт на основе клеточных автоматов.
    /// </summary>
    internal sealed class RegionDraft
    {
        private readonly HashSet<OffsetCoords> _coords;

        /// <summary>
        /// Конструктор черновика региона карты.
        /// </summary>
        /// <param name="coords"></param>
        public RegionDraft(IEnumerable<OffsetCoords> coords)
        {
            if (coords is null)
            {
                throw new ArgumentNullException(nameof(coords));
            }

            _coords = new HashSet<OffsetCoords>(coords);
        }

        /// <summary>
        /// Координаты, которые входя в черновик региона.
        /// </summary>
        public IEnumerable<OffsetCoords> Coords => _coords;

        public bool Contains(OffsetCoords coords)
        {
            return _coords.Contains(coords);
        }
    }
}