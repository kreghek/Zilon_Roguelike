using Newtonsoft.Json;

namespace Zilon.Core.Schemes
{
    public sealed record PropEquipRestrictions : IPropEquipRestrictions
    {
        [JsonProperty]
        public PropHandUsage? PropHandUsage { get; init; }
    }
}