namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Схема инструментов, требуемых для крафта.
    /// </summary>
    public sealed class InstrumentLevelFunctionSubScheme: SubSchemeBase
    {
        /// <summary>
        /// Функциональное назначение инструментов.
        /// </summary>
        public InstrumentFunctions Function { get; set; }

        /// <summary>
        /// Уровень функционального назначения.
        /// </summary>
        public int Level { get; set; }
    }
}
