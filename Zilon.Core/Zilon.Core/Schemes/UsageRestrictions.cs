using Newtonsoft.Json;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Base implemenetation of restriction set.
    /// </summary>
    public sealed class UsageRestrictions : IUsageRestrictions
    {
        /// <inheritdoc />
        [JsonProperty]
        [JsonConverter(typeof(ConcreteTypeConverter<UsageRestrictionItem[]>))]
        public IUsageRestrictionItem?[]? Items { get; private set; }
    }
}