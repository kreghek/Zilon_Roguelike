using Newtonsoft.Json;

using Zilon.Core.Components;

namespace Zilon.Core.Schemes
{
    public sealed class ConsumeCommonRule
    {
        [JsonConstructor]
        public ConsumeCommonRule(ConsumeCommonRuleType type, PersonRuleLevel level)
        {
            Type = type;
            Level = level;
        }

        public ConsumeCommonRuleType Type { get; }
        public PersonRuleLevel Level { get; }
    }
}
