using System;

using Zilon.Core.Components;
using Zilon.Core.Diseases;

namespace Zilon.Core.Persons.Survival
{
    public class DiseaseEffect : IPersonEffect
    {
        public DiseaseEffect(IDisease disease)
        {
            Disease = disease;
        }

        public IDisease Disease { get; }

        public event EventHandler Changed;

        public EffectRule[] GetRules()
        {
            return new[] { new EffectRule(RollEffectType.Efficient, PersonRuleLevel.Lesser) };
        }
    }
}
