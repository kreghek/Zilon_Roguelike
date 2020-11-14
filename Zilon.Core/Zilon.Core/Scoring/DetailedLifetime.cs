namespace Zilon.Core.Scoring
{
    /// <summary>
    ///     Структура для хранения разобранного значения времени жизни персонажа.
    /// </summary>
    public sealed class DetailedLifetime
    {
        /// <summary>
        ///     Конструктор объекта.
        /// </summary>
        /// <param name="days"> Количество целых прожитых дней. </param>
        /// <param name="hours"> Количество целых пожитых часов. </param>
        public DetailedLifetime(int days, int hours)
        {
            Days = days;
            Hours = hours;
        }

        /// <summary>
        ///     Количество целых прожитых дней.
        /// </summary>
        public int Days { get; }

        /// <summary>
        ///     Количество целых пожитых часов.
        /// </summary>
        public int Hours { get; }
    }
}