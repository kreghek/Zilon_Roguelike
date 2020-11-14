namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Схема действия.
    /// </summary>
    public interface ITacticalActScheme : IScheme, IMimicScheme
    {
        /// <summary>
        /// Ограничения на использование действия.
        /// </summary>
        ITacticalActConstrainsSubScheme Constrains { get; }

        /// <summary>
        /// Основные характеристики действия.
        /// </summary>
        ITacticalActStatsSubScheme Stats { get; }
    }
}