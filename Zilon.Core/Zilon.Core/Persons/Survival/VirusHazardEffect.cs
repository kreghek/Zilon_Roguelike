using System;

using Zilon.Core.Components;

namespace Zilon.Core.Persons.Survival
{
    public class VirusHazardEffect : IPersonEffect
    {
        public VirusHazardEffect(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string Name { get; }

        public event EventHandler Changed;

        public EffectRule[] GetRules()
        {
            return new[] { new EffectRule(RollEffectType.Efficient, PersonRuleLevel.Lesser) };
        }
    }
}
