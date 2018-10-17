namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Общие правила влияния поглощения предмета.
    /// </summary>
    public enum ConsumeCommonRuleType
    {
        Undefined,

        /// <summary>
        /// Влияние на сытость персонажа.
        /// </summary>
        Satiety,

        /// <summary>
        /// Влияние на жажду персонажа.
        /// </summary>
        Thrist,

        /// <summary>
        /// Влияет на здоровье.
        /// </summary>
        Health
    }
}
