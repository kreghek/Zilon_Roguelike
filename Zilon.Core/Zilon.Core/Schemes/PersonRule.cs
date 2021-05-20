﻿using Newtonsoft.Json;

using Zilon.Core.Components;

namespace Zilon.Core.Schemes
{
    public sealed class PersonRule
    {
        [JsonConstructor]
        public PersonRule(EquipCommonRuleType type, PersonRuleLevel level,
            PersonRuleDirection direction = PersonRuleDirection.Positive)
        {
            Type = type;
            Level = level;
            Direction = direction;
        }

        public PersonRuleDirection Direction { get; }
        public PersonRuleLevel Level { get; }

        public EquipCommonRuleType Type { get; }
    }
}