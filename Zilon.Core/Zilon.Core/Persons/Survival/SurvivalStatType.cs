namespace Zilon.Core.Persons
{
    /// <summary>
    /// Типы характеристик для модуля выживания.
    /// </summary>
    public enum SurvivalStatType
    {
        /// <summary>
        /// Не определена. Скорее всего, ошибка.
        /// </summary>
        Undefined,

        /// <summary>
        /// Хитпоинты.
        /// </summary>
        Health,

        /// <summary>
        /// Сытость.
        /// </summary>
        Satiety,

        /// <summary>
        /// Достаточность воды. Напоенность, упитость.
        /// </summary>
        Hydration,

        /// <summary>
        /// Интоксикация персонажа.
        /// </summary>
        Intoxication
    }
}
