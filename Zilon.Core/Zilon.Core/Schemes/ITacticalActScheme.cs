namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Схема действия.
    /// </summary>
    public interface ITacticalActScheme: IScheme
    {
        /// <summary>
        /// Ограничения на использование действия.
        /// </summary>
        TacticalActConstrainsSubScheme Constrains { get; }

        /// <summary>
        /// Основные характеристики действия.
        /// </summary>
        ITacticalActStatsSubScheme Stats { get; }
    }
}