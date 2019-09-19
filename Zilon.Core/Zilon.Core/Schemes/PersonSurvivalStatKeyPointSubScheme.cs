using Newtonsoft.Json;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Базовая реализация подсхемы ключевой точки характеристики выживания.
    /// </summary>
    public sealed class PersonSurvivalStatKeyPointSubScheme : IPersonSurvivalStatKeyPointSubScheme
    {
        [JsonProperty]
        public PersonSurvivalStatKeypointLevel Level { get; private set; }

        [JsonProperty]
        public int Start { get; private set; }
    }
}
