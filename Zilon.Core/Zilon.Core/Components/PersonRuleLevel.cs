namespace Zilon.Core.Components
{
    /// <summary>
    /// Степень правила персонажа.
    /// </summary>
    public enum PersonRuleLevel
    {
        /// <summary>
        /// Правило не влияет.
        /// </summary>
        None = 0,

        /// <summary>
        /// Малое влияние правила.
        /// </summary>
        Lesser = 10,

        /// <summary>
        /// Обычное влияние правила.
        /// </summary>
        Normal = 20,

        /// <summary>
        /// Великое влияние правила.
        /// </summary>
        Grand = 30,

        /// <summary>
        /// Абсолютное правило.
        /// </summary>
        Absolute = 100
    }
}
