using Newtonsoft.Json;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Схема персонажа.
    /// </summary>
    public class PersonScheme : SchemeBase, IPersonScheme
    {
        /// <inheritdoc/>
        public int Hp { get; set; }

        /// <inheritdoc/>
        public PersonSlotSubScheme?[]? Slots { get; set; }

        /// <inheritdoc/>
        [JsonConverter(typeof(ConcreteTypeConverter<PersonSurvivalStatSubScheme[]>))]
        [JsonProperty]
        public IPersonSurvivalStatSubScheme?[]? SurvivalStats { get; private set; }

        /// <inheritdoc/>
        public string?[]? DefaultActs { get; set; }
    }
}