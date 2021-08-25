using Newtonsoft.Json;

namespace Zilon.Core.Schemes
{
    public sealed class PropEquipRestrictions : IPropEquipRestrictions
    {
        [JsonProperty]
        public PropHandUsage? PropHandUsage { get; init; }
    }
}