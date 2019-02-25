using Newtonsoft.Json;

using Zilon.Core.Components;

namespace Zilon.Core.Schemes
{
    public sealed class EquipCommonRule
    {
        [JsonConstructor]
        public EquipCommonRule(EquipCommonRuleType type, PersonRuleLevel level)
        {
            Type = type;
            Level = level;
        }

        public EquipCommonRuleType Type { get; }
        public PersonRuleLevel Level { get; }
    }
}
