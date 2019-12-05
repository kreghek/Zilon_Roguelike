namespace Zilon.Core.MapGenerators.RoomStyle
{
    /// <summary>
    /// Метаданные элемета интерьера комнаты.
    /// </summary>
    /// <remarks>
    /// Используется для генерации карт на основе комнат.
    /// </remarks>
    public sealed class RoomInteriorObjectMeta
    {
        /// <summary>
        /// Конструирует объект <see cref="RoomInteriorObjectMeta"/>.
        /// </summary>
        /// <param name="coords">Локальные координаты элемента интерьера внутри комнаты.</param>
        /// <exception cref="System.ArgumentNullException">coords</exception>
        /// <remarks>
        /// Это локальны координаты. Т.е. (0, 0) - это левый нижний угол комнаты, а не карты.
        /// </remarks>
        public RoomInteriorObjectMeta(OffsetCoords coords)
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
