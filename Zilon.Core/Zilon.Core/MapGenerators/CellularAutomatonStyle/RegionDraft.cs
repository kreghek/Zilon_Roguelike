using System;

namespace Zilon.Core.MapGenerators.CellularAutomatonStyle
{
    /// <summary>
    /// Черновик региона карты.
    /// Промежуточная структура, которая используется для заливки
    /// кластеров в фабрике карт на основе клеточных автоматов.
    /// </summary>
    internal sealed class RegionDraft
    {
        /// <summary>
        /// Конструктор черновика региона карты.
        /// </summary>
        /// <param name="coords"></param>
        public RegionDraft(OffsetCoords[] coords)
        {
            Coords = coords ?? throw new ArgumentNullException(nameof(coords));
        }

        /// <summary>
        /// Координаты, которые входя в черновик региона.
        /// </summary>
        public OffsetCoords[] Coords { get; }
    }
}
