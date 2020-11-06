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
        /// <inheritdoc/>
        [JsonProperty]
        public int? PropResourceCount { get; private set; }

        /// <inheritdoc/>
        [JsonProperty]
        public string PropResourceType { get; private set; }

        /// <inheritdoc/>
        [JsonProperty]
        public int? Cooldown { get; private set; }
    }
}