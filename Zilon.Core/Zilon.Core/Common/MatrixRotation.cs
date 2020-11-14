namespace Zilon.Core.Common
{
    /// <summary>
    /// Доступные повороты матрицы.
    /// </summary>
    public enum MatrixRotation
    {
        /// <summary>
        /// Не поворачивать.
        /// </summary>
        None = 0,

        /// <summary>
        /// Повернуть на 90 градусов по часовой.
        /// </summary>
        Clockwise90 = 1,

        /// <summary>
        /// Развернуть на противоположную сторону.
        /// </summary>
        Conter180 = 2,

        /// <summary>
        /// Повернуть на 90 градусов против часовой.
        /// </summary>
        ConterClockwise90 = 3
    }
}