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
        public string[] RegularMonsterSids { get; private set; }

        /// <summary>
        /// Идентификаторы редких монстров, встречаемых в секторе.
        /// </summary>
        [JsonProperty]
        public string[] RareMonsterSids { get; private set; }

        /// <summary>
        /// Идентификаторы боссов, встречаемых в секторе.
        /// </summary>
        [JsonProperty]
        public string[] ChampionMonsterSids { get; private set; }


        /// <summary>
        /// Тип фабрики для карты.
        /// </summary>
        [JsonProperty]
        public SectorSubSchemeMapFactory MapFactory { get; private set; }

        /// <summary>
        /// Количество регионов в карте.
        /// </summary>
        /// <remarks>
        /// Для подземелий это количество комнат.
        /// </remarks>
        [JsonProperty]
        public int RegionCount { get; private set; }

        /// <summary>
        /// Максимальный размер комнат.
        /// </summary>
        /// <remarks>
        /// Минимальный размер всегда 2х2.
        /// </remarks>
        [JsonProperty]
        public int RegionSize { get; private set; }

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
        public string[] TransSectorSids { get; private set; }

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
        public SchemeSectorMapGenerator MapGenerator { get; private set; }
    }
}
