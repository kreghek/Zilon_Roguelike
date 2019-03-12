namespace Zilon.Core.Persons
{
    /// <summary>
    /// Ключевое значение характеристики выживания.
    /// </summary>
    public sealed class SurvivalStatKeyPoint
    {
        public SurvivalStatKeyPoint(SurvivalStatHazardLevel level, int value)
        {
            Level = level;
            Value = value;
        }

        /// <summary>Уровень влияния на характеристику.</summary>
        /// <remarks>
        /// Например, при указанном значении наступит сильный голод.
        /// </remarks>
        public SurvivalStatHazardLevel Level { get; }

        /// <summary>Значение ключевой точки.</summary>
        public int Value { get; }

        public override string ToString()
        {
            return $"{Level}: {Value}";
        }
    }
}
