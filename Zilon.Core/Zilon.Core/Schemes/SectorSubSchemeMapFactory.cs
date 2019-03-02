namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Возможные фабрики генерации карт.
    /// </summary>
    public enum SectorSubSchemeMapFactory
    {
        /// <summary>
        /// Квадратная карта.
        /// </summary>
        /// <remarks>
        /// Будет использоваться для секторов в дикой местности (сейчас только лес).
        /// </remarks>
        Square = 1,

        /// <summary>
        /// Карта на основе комнат.
        /// </summary>
        /// <remarks>
        /// Текущий генератор, который строит подземелья <see cref="MapGenerators.RoomStyle.RoomMapFactory"/>.
        /// </remarks>
        Rooms
    }
}
