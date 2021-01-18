using Newtonsoft.Json;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Подсхема предмета для хранения характеристик при применении предмета.
    /// </summary>
    public class PropUseSubScheme : SubSchemeBase, IPropUseSubScheme
    {
        /// <inheritdoc/>
        [JsonProperty]
        public bool Consumable { get; private set; }

        /// <inheritdoc/>
        [JsonProperty]
        public ConsumeCommonRule[] CommonRules { get; private set; }

        /// <inheritdoc/>
        [JsonProperty]
        public IUsageRestrictions Restrictions { get; private set; }
    }
}