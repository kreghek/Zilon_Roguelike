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
        HealthIfNoBody,

        /// <summary>
        /// Влияет на шанс снижения характеристики - сытость.
        /// </summary>
        HungerResistance,

        /// <summary>
        /// Влияет на шанс снижения характеристики - вода.
        /// </summary>
        ThristResistance
    }
}