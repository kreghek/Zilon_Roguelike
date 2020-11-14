namespace Zilon.Core.Persons.Survival
{
    /// <summary>
    /// Ключевой сегмент характеристики выживания.
    /// </summary>
    public sealed class SurvivalStatKeySegment
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="start">Начальное значение сегмента в долях от 0.0f до 1.0f.</param>
        /// <param name="end">Конечное значение сегмента в долях от 0.0f до 1.0f.</param>
        /// <param name="level">Уровень влияния на характеристику.</param>
        public SurvivalStatKeySegment(float start, float end, SurvivalStatHazardLevel level)
        {
            if (start > end)
            {
                throw new ArgumentException("Начальная точка должна быть меньше или равно конечной точки.");
            }

            Start = start;
            End = end;
            Level = level;
        }

        /// <summary>
        /// Начальное значение сегмента в долях от 0.0f до 1.0f.
        /// </summary>
        public float Start { get; }

        /// <summary>
        /// Конечное значение сегмента в долях от 0.0f до 1.0f.
        /// </summary>
        public float End { get; }

        /// <summary>
        /// Уровень влияния на характеристику.
        /// </summary>
        /// <remarks>
        /// Например, если значение попадает в диапазон [Start..End], то наступит сильный голод.
        /// </remarks>
        public SurvivalStatHazardLevel Level { get; }
    }
}