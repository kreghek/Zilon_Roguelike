namespace Zilon.Core.WorldGeneration
{
    /// <summary>
    /// Специализация населения.
    /// </summary>
    public enum PopulationSpecializations
    {
        /// <summary>
        /// Не определено. Если это значение, значит, скорее всего, ошибка.
        /// Сейчас не предполагается, что население будет без специализации.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// Рабочие.
        /// </summary>
        Workers,

        /// <summary>
        /// Крестьяне.
        /// </summary>
        Peasants,

        /// <summary>
        /// Учёные.
        /// </summary>
        Scientists,

        /// <summary>
        /// Служащие.
        /// </summary>
        Servants
    }
}
