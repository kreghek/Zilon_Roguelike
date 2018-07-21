namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Требуемые профессиональные компетенции.
    /// </summary>
    public sealed class ProfessionRequirementSubScheme: SubSchemeBase
    {
        /// <summary>
        /// Тип компетенции.
        /// </summary>
        public ProfessionTypes Profession { get; set; }

        /// <summary>
        /// Минимальный уровень владения.
        /// </summary>
        public int MinLevel { get; set; }

        /// <summary>
        /// Максимальный уровень владения.
        /// </summary>
        public int MaxLevel { get; set; }
    }
}
