using Newtonsoft.Json;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Базовая реализация подсхемы характеристики выживания персонажа.
    /// </summary>
    public sealed class PersonSurvivalStatSubScheme : SubSchemeBase, IPersonSurvivalStatSubScheme
    {
        [JsonProperty]
        public PersonSurvivalStatType Type { get; private set; }

        [JsonProperty]
        [JsonConverter(typeof(ConcreteTypeConverter<PersonSurvivalStatKeySegmentSubScheme[]>))]
        public IPersonSurvivalStatKeySegmentSubScheme[] KeyPoints { get; private set; }

        [JsonProperty]
        public int StartValue { get; private set; }

        [JsonProperty]
        public int MinValue { get; private set; }

        [JsonProperty]
        public int MaxValue { get; private set; }
    }
}
