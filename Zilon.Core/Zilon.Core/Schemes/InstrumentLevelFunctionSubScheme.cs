namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Схема инструментов, требуемых для крафта.
    /// </summary>
    public sealed class InstrumentLevelFunctionSubScheme : SubSchemeBase
    {
        /// <summary>
        /// Функциональное назначение инструментов.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public InstrumentFunctions Function { get; set; }

        /// <summary>
        /// Уровень функционального назначения.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public int Level { get; set; }
    }
}