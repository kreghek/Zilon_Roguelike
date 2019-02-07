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

        /// <summary>
        /// Количество сундуков в секторе.
        /// </summary>
        [JsonProperty]
        public int ChestCount { get; private set; }

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
    }
}
