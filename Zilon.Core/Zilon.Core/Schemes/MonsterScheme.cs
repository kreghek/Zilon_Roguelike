namespace Zilon.Core.Schemes
{
    using Newtonsoft.Json;

    /// <summary>
    /// Схема монстра.
    /// </summary>
    public sealed class MonsterScheme : SchemeBase, IMonsterScheme
    {
        /// <summary>
        /// Базовые очки, начисляемые за убиство монстра.
        /// </summary>
        [JsonProperty]
        public int BaseScore { get; private set; }

        /// <summary>
        /// Способности к обороне монстра против атакующих действий противника.
        /// </summary>
        [JsonConverter(typeof(ConcreteTypeConverter<MonsterDefenceSubScheme>))]
        [JsonProperty]
        public IMonsterDefenseSubScheme Defense { get; private set; }

        /// <summary>
        /// Список идентификаторов таблиц дропа.
        /// </summary>
        [JsonProperty]
        public string[] DropTableSids { get; private set; }

        /// <summary>
        /// Хитпоинты монстра.
        /// </summary>
        [JsonProperty]
        public int Hp { get; private set; }

        /// <summary>
        /// Основное действие монстра.
        /// </summary>
        [JsonConverter(typeof(ConcreteTypeConverter<TacticalActStatsSubScheme>))]
        [JsonProperty]
        public ITacticalActStatsSubScheme PrimaryAct { get; private set; }

        /// <inheritdoc />
        [JsonProperty]
        public string[] Tags { get; private set; }
    }
}