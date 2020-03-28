using Newtonsoft.Json;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Реализация схемы сектора.
    /// </summary>
    public sealed class SectorSubScheme : SubSchemeBase, ISectorSubScheme
    {
        /// <summary>
        /// Идентфиикаторы обычных монстров, встречаемых в секторе.
        /// </summary>
        [JsonProperty]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:Properties should not return arrays",
            Justification = "Используется для десериализации")]
        public string[] RegularMonsterSids { get; private set; }

        /// <summary>
        /// Идентификаторы редких монстров, встречаемых в секторе.
        /// </summary>
        [JsonProperty]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:Properties should not return arrays",
            Justification = "Используется для десериализации")]
        public string[] RareMonsterSids { get; private set; }

        /// <summary>
        /// Идентификаторы боссов, встречаемых в секторе.
        /// </summary>
        [JsonProperty]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:Properties should not return arrays",
            Justification = "Используется для десериализации")]
        public string[] ChampionMonsterSids { get; private set; }

        /// <summary>
        /// Количество монстров в секторе.
        /// </summary>
        [JsonProperty]
        public int RegionMonsterCount { get; private set; }

        [JsonProperty]
        public int MinRegionMonsterCount { get; private set; }

        /// <summary>
        /// Количество сундуков в секторе.
        /// </summary>
        [JsonProperty]
        public int TotalChestCount { get; private set; }

        /// <summary>
        /// Таблицы дропа для сундуков.
        /// </summary>
        [JsonProperty]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:Properties should not return arrays",
            Justification = "Используется для десериализации")]
        public string[] ChestDropTableSids { get; private set; }


        /// <summary>
        /// Символьный идентфиикатор сектора.
        /// </summary>
        /// <remarks>
        /// Нужен для перехода из сектора в сектор.
        /// </remarks>
        [JsonProperty]
        public string Sid { get; private set; }

        /// <summary>
        /// Наименование сектора.
        /// </summary>
        [JsonProperty]
        public LocalizedStringSubScheme Name { get; private set; }

        /// <summary>
        /// Описание сектора.
        /// </summary>
        [JsonProperty]
        public LocalizedStringSubScheme Description { get; private set; }

        /// <summary>
        /// Идентфикаторы связанных секторов в рамках текущей локации.
        /// </summary>
        /// <seealso cref="Sid"/>
        [JsonProperty]
        [JsonConverter(typeof(ConcreteTypeConverter<SectorTransitionSubScheme[]>))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:Properties should not return arrays",
            Justification = "Используется для десериализации")]
        public ISectorTransitionSubScheme[] TransSectorSids { get; private set; }

        /// <summary>
        /// Индикатор того, что сектор является стартовым при входе из локации.
        /// </summary>
        [JsonProperty]
        public bool IsStart { get; private set; }

        /// <summary>
        /// Коэффициент максимального количества сундуков в регионе сектора (комнате)
        /// в зависимости от размера региона.
        /// </summary>
        [JsonProperty]
        public int RegionChestCountRatio { get; private set; }

        [JsonProperty]
        [JsonConverter(typeof(ConcreteTypeConverter<SectorMapFactoryOptionsSubSchemeBase>))]
        public ISectorMapFactoryOptionsSubScheme MapGeneratorOptions { get; private set; }
    }
}
