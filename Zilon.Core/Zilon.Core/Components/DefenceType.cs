namespace Zilon.Core.Components
{
    [JsonConverter(typeof(StringEnumConverter))]
    [PublicAPI]
    public enum DefenceType
    {
        /// <summary>
        /// Неопределённое.
        /// </summary>
        /// <remarks>
        /// Если выбрано, то, скорее всего, ошибка.
        /// </remarks>
        Undefined,

        TacticalDefence = 10,
        FuryDefence = 20,
        ShadowDefence = 30,
        TrickyDefence = 40,
        ConcentratedDefence = 50,
        RapidDefence = 60,

        /// <summary>
        /// Даёт бонус против всех типов наступления. Кроме божественного.
        /// </summary>
        DivineDefence = 100
    }
}