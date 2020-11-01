namespace Zilon.Core.Schemes
{
    /// <summary>
    ///     Базовая реализация подсхемы характеристики выживания персонажа.
    /// </summary>
    public sealed class PersonSurvivalStatSubScheme : SubSchemeBase, IPersonSurvivalStatSubScheme
    {
        /// <inheritdoc />
        [JsonProperty]
        public PersonSurvivalStatType Type { get; private set; }

        /// <inheritdoc />
        [JsonProperty]
        [JsonConverter(typeof(ConcreteTypeConverter<PersonSurvivalStatKeySegmentSubScheme[]>))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:Properties should not return arrays",
            Justification = "Реализация интерфейса")]
        public IPersonSurvivalStatKeySegmentSubScheme[] KeyPoints { get; private set; }

        /// <inheritdoc />
        [JsonProperty]
        public int StartValue { get; private set; }

        /// <inheritdoc />
        [JsonProperty]
        public int MinValue { get; private set; }

        /// <inheritdoc />
        [JsonProperty]
        public int MaxValue { get; private set; }

        /// <inheritdoc />
        [JsonProperty]
        public int? DownPassRoll { get; private set; }
    }
}