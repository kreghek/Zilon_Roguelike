using Newtonsoft.Json;

namespace Zilon.Core.Schemes
{
    public sealed class SectorTransitionSubScheme : ISectorTransitionSubScheme
    {
        [JsonProperty] public string SectorLevelSid { get; private set; }
    }
}