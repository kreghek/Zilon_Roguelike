using Newtonsoft.Json;

using Zilon.Core.Components;

namespace Zilon.Core.Schemes
{
    public sealed class ConsumeCommonRule
    {
        
        public ConsumeCommonRule(ConsumeCommonRuleType type, PersonRuleLevel level, PersonRuleDirection direction)
        {
            Type = type;
            Level = level;
            Direction = direction;
        }
        
        [JsonConstructor]
        public ConsumeCommonRule(ConsumeCommonRuleType type, PersonRuleLevel level) : this(type, level, PersonRuleDirection.Positive)
        { }

        public ConsumeCommonRuleType Type { get; }
        public PersonRuleLevel Level { get; }
        public PersonRuleDirection Direction { get; }
    }
}
