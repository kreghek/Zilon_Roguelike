namespace Zilon.Core.WorldGeneration
{
    /// <summary>
    /// Единица населения города или группы мигрантов.
    /// Условно одна единица популяции - это 100 человек.
    /// </summary>
    public sealed class Population
    {
        /// <summary>
        /// Специализация населения.
        /// </summary>
        public PopulationSpecializations Specialization { get; set; }
    }
}
