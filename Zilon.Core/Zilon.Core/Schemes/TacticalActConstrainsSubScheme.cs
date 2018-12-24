using Newtonsoft.Json;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Подсхема для хранения ограничений на использование действия.
    /// </summary>
    /// <remarks>
    /// Используется только актёрами под прямым управлением игрока.
    /// </remarks>
    public class TacticalActConstrainsSubScheme : SubSchemeBase, ITacticalActConstrainsSubScheme
    {
        [JsonProperty]
        public int? PropResourceCount { get; private set; }

        [JsonProperty]
        public string PropResourceType { get; private set; }
    }
}
