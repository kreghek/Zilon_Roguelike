namespace Zilon.Core.Components
{
    [PublicAPI]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OffenseType
    {
        /// <summary>
        /// Неопределённое.
        /// </summary>
        /// <remarks>
        /// Если выбрано, то, скорее всего, ошибка.
        /// </remarks>
        Undefined,

        Tactical = 10,
        Fury = 20,
        Shadow = 30,
        Tricky = 40,
        Concentrated = 50,
        Rapid = 60,

        /// <summary>
        /// Божественный. Бонусы других классов защиты не дают явного бонуса. Кроме божественной защиты.
        /// </summary>
        Divine = 100
    }
}