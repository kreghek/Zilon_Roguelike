namespace Zilon.Core.Schemes
{
    /// <inheritdoc />
    /// <summary>
    /// Схема действия.
    /// </summary>
    public class TacticalActScheme: SchemeBase
    {
        /// <summary>
        /// Основные характеристики действия.
        /// </summary>
        public TacticalActStatsSubScheme Stats { get; set; }

        /// <summary>
        /// Ограничения на использование действия.
        /// </summary>
        public TacticalActConstrainsSubScheme Constrains { get; set; }

        /// <summary>
        /// Зависимости действия от характеристик.
        /// </summary>
        public TacticalActDependencySubScheme[] Dependency { get; set; }
    }
}
