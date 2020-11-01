namespace Zilon.Core.Persons
{
    /// <summary>
    ///     Типы характеристик для модуля выживания.
    /// </summary>
    public enum SurvivalStatType
    {
        /// <summary>
        ///     Не определена. Скорее всего, ошибка.
        /// </summary>
        Undefined,

        /// <summary>
        ///     Хитпоинты.
        /// </summary>
        Health,

        /// <summary>
        ///     Сытость.
        /// </summary>
        Satiety,

        /// <summary>
        ///     Достаточность воды. Напоенность, упитость.
        /// </summary>
        Hydration,

        /// <summary>
        ///     Интоксикация персонажа.
        /// </summary>
        Intoxication,

        /// <summary>
        ///     Текущее состояние раны.
        ///     Используется для регенрации здоровья.
        ///     Введено, потому что здоровья мало, оно целочисленное, а регенерация должна быть медленной.
        /// </summary>
        Wound,

        /// <summary>
        ///     Достаточность воздуха для дыхания.
        /// </summary>
        Breath,

        /// <summary>
        ///     Энергия персонажа.
        ///     Если энергия кончается, персонаж устаёт.
        /// </summary>
        Energy
    }
}