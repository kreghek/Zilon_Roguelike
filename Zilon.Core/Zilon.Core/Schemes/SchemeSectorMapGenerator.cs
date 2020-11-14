namespace Zilon.Core.Schemes
{
    /// <summary>
    ///     Возможные генераторы карт для секторов.
    /// </summary>
    public enum SchemeSectorMapGenerator
    {
        /// <summary>
        ///     Не определён.
        /// </summary>
        /// <remarks>
        ///     Для всех схем секторов должно быть указано,
        ///     каким генератором создавать карту.
        ///     Если не указано, то это считается ошибкой.
        /// </remarks>
        Undefined = 0,

        /// <summary>
        ///     Генерация на основе комнат.
        /// </summary>
        Room,

        /// <summary>
        ///     Генерация на основе клеточного автомата.
        /// </summary>
        CellularAutomaton,

        /// <summary>
        ///     Генерация квадратной монолитной карты.
        /// </summary>
        SquarePlane
    }
}