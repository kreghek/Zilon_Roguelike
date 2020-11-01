namespace Zilon.Core.Schemes
{
    /// <summary>
    ///     Подсхема для описания ключевых сегментов какой-либо характеристики выживания.
    /// </summary>
    /// <remarks>
    ///     Ключевые сегменты будут расположены в порядке возрастания.
    ///     Они гарантировано не пересекаются.
    ///     Начальное значение меньше или равно конечному.
    /// </remarks>
    //TODO Проверять корректность в тестах схем.
    public interface IPersonSurvivalStatKeySegmentSubScheme
    {
        /// <summary>
        ///     Уровень ключевой точки.
        /// </summary>
        PersonSurvivalStatKeypointLevel Level { get; }

        /// <summary>
        ///     Начальное значение ключевого сегмента характеристики выживания.
        /// </summary>
        float Start { get; }

        /// <summary>
        ///     Конечное значение ключевого сегмента характеристики выживания.
        /// </summary>
        float End { get; }
    }
}