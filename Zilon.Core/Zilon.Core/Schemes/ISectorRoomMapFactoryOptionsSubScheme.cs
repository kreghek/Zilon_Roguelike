namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Схема параметров генерации карты на основе клеточного автомата.
    /// </summary>
    public interface ISectorRoomMapFactoryOptionsSubScheme : ISectorMapFactoryOptionsSubScheme
    {
        /// <summary>
        /// Количество регионов в карте.
        /// </summary>
        /// <remarks>
        /// Для подземелий это количество комнат.
        /// </remarks>
        int RegionCount { get; }

        /// <summary>
        /// Максимальный размер комнат.
        /// </summary>
        /// <remarks>
        /// Минимальный размер всегда 2х2.
        /// </remarks>
        int RegionSize { get; }
    }
}