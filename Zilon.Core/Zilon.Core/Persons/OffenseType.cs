namespace Zilon.Core.Persons
{
    public enum OffenseType
    {
        /// <summary>
        /// Неопределённое.
        /// </summary>
        /// <remarks>
        /// Если выбрано, то, скорее всего, ошибка.
        /// </remarks>
        Undefined,

        Tactical,
        Fury,
        Shadow,
        Tricky,
        Force,
        Rapid,

        /// <summary>
        /// Божественный. Бонусы других классов защиты не дают явного бонуса. Кроме божественной защиты.
        /// </summary>
        Divine
    }
}
