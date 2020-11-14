using Newtonsoft.Json;
using Zilon.Core.Common;
using Zilon.Core.Components;

namespace Zilon.Core.Schemes
{
    public class PropArmorItemSubScheme : SubSchemeBase, IPropArmorItemSubScheme
    {
        [JsonProperty] public ImpactType Impact { get; private set; }

        [JsonProperty] public PersonRuleLevel AbsorbtionLevel { get; private set; }

        [JsonProperty] public int ArmorRank { get; private set; }
    }
}