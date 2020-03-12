using System;

using Newtonsoft.Json;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Схема действия.
    /// </summary>
    public class TacticalActScheme : SchemeBase, ITacticalActScheme
    {
        /// <inheritdoc/>
        [JsonProperty]
        public ITacticalActStatsSubScheme Stats { get; }

        /// <inheritdoc/>
        [JsonProperty]
        public ITacticalActConstrainsSubScheme Constrains { get; }

        /// <inheritdoc/>
        [JsonProperty]
        public string IsMimicFor { get; private set; }
    }
}
