namespace Zilon.Core.Components
{
    [PublicAPI]
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum SkillStatType
    {
        /// <summary>
        ///     Неопределённый навык. Используется только монстрами.
        /// </summary>
        Undefined,

        /// <summary>
        ///     Навык рукопашного боя.
        /// </summary>
        Melee,

        /// <summary>
        ///     Навык стрельбы.
        /// </summary>
        Ballistic,

        /// <summary>
        ///     Знание техники.
        /// </summary>
        /// <remarks>
        ///     Влияет применение действий, требующих использование сложных
        ///     технических устройств.
        /// </remarks>
        Tech,

        /// <summary>
        ///     Медицина.
        /// </summary>
        Medic,

        /// <summary>
        ///     Псионические способности.
        /// </summary>
        /// <remarks>
        ///     Магия, гипноз и т.д.
        /// </remarks>
        Psy,

        /// <summary>
        ///     Понимание социума.
        /// </summary>
        /// <remarks>
        ///     Влияет на действия, требующих влияние на индивидуумов общества.
        ///     Например, страх или провакация.
        /// </remarks>
        Social
    }
}