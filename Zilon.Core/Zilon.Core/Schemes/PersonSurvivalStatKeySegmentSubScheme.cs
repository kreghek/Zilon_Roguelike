using Newtonsoft.Json;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Базовая реализация подсхемы ключевой точки характеристики выживания.
    /// </summary>
    public sealed class PersonSurvivalStatKeySegmentSubScheme : IPersonSurvivalStatKeySegmentSubScheme
    {
        [JsonProperty]
        public PersonSurvivalStatKeypointLevel Level { get; private set; }

        [JsonProperty]
        public float Start { get; private set; }

        [JsonProperty]
        public float End { get; private set; }
    }
}