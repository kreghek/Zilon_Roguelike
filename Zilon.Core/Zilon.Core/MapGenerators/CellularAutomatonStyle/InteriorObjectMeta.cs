using System;

namespace Zilon.Core.MapGenerators.CellularAutomatonStyle
{
    /// <summary>
    /// Метаданные элемета интерьера комнаты.
    /// </summary>
    /// <remarks>
    /// Используется для генерации карт на основе комнат.
    /// </remarks>
    public sealed class InteriorObjectMeta
    {
        /// <summary>
        /// Конструирует объект <see cref="InteriorObjectMeta" />.
        /// </summary>
        /// <param name="coords">Локальные координаты элемента интерьера внутри комнаты.</param>
        /// <exception cref="ArgumentNullException">coords</exception>
        /// <remarks>
        /// Это локальны координаты. Т.е. (0, 0) - это левый нижний угол комнаты, а не карты.
        /// </remarks>
        public InteriorObjectMeta(OffsetCoords coords)
        {
            Coords = coords;
        }

        /// <summary>
        /// Координаты элемента интерьера внутри комнаты.
        /// </summary>
        /// <remarks>
        /// Это локальны координаты. Т.е. (0, 0) - это левый нижний угол комнаты, а не карты.
        /// </remarks>
        public OffsetCoords Coords { get; }
    }
}