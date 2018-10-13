namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Схема действия.
    /// </summary>
    public class TacticalActScheme : SchemeBase, ITacticalActScheme
    {
        /// <summary>
        /// Основные характеристики действия.
        /// </summary>
        public ITacticalActStatsSubScheme Stats { get; }

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
