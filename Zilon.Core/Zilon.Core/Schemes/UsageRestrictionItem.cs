using Newtonsoft.Json;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Base implementation of <see cref="IUsageRestrictionItem" />
    /// </summary>
    public sealed class UsageRestrictionItem : IUsageRestrictionItem
    {
        /// <inheritdoc />
        [JsonProperty]
        public UsageRestrictionRule Type { get; private set; }
    }
}