namespace Zilon.Core.WorldGeneration
{
    /// <summary>
    /// Реализация события в городе.
    /// </summary>
    public sealed class LocalityEvent
    {
        /// <summary>
        /// Карточка, которая отвечает за событие.
        /// </summary>
        public ILocalityEventCard EventCard { get; set; }

        /// <summary>
        /// Счётчик события.
        /// В зависимости от события счётчик влияет на степень события (при голоде, море, эпидемии со временем начинает вымирать население).
        /// </summary>
        public int Counter { get; set; }
    }
}
