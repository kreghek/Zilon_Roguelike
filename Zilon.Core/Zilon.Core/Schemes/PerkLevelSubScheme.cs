using Zilon.Core.Components;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Уровень развития перка.
    /// </summary>
    /// <remarks>
    /// У одного перка может быть несколько уровней развития.
    /// Можно считать, что каждый уровень это одтельный перк с таким же названием
    /// и дополнительными условия для развития.
    /// </remarks>
    public sealed class PerkLevelSubScheme : SubSchemeBase
    {
        public PerkRuleSubScheme[] Rules { get; set; }

        [JsonConverter(typeof(ConcreteTypeConverter<JobSubScheme[]>))]
        [JsonProperty]
        public IJobSubScheme[] Jobs { get; set; }

        public int MaxValue { get; set; }

        public PersonStat PersonLevel { get; set; }

        public PerkConditionSubScheme[] Conditions { get; set; }

        public PropSet[] Sources { get; set; }
    }
}