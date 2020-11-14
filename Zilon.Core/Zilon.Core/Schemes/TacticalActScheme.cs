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
        [JsonConverter(typeof(ConcreteTypeConverter<TacticalActStatsSubScheme>))]
        public ITacticalActStatsSubScheme Stats { get; private set; }

        /// <inheritdoc/>
        [JsonProperty]
        [JsonConverter(typeof(ConcreteTypeConverter<TacticalActConstrainsSubScheme>))]
        public ITacticalActConstrainsSubScheme Constrains { get; private set; }

        /// <inheritdoc/>
        [JsonProperty]
        public string IsMimicFor { get; private set; }
    }
}