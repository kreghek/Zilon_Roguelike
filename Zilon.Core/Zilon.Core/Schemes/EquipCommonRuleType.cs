namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Общие правила влияния экипировки предмета.
    /// </summary>
    public enum EquipCommonRuleType
    {
        /// <summary>
        /// Влияет на здоровье.
        /// </summary>
        Health = 1,

        /// <summary>
        /// Влияет на здоровье, если нет доспеха на тело.
        /// </summary>
        HealthIfNoBody
    }
}
