using System.Diagnostics.CodeAnalysis;

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
        [ExcludeFromCodeCoverage]
        public PerkRuleSubScheme[] Rules { get; set; }

        [ExcludeFromCodeCoverage]
        public JobSubScheme[] Jobs { get; set; }

        [ExcludeFromCodeCoverage]
        public int MaxValue { get; set; }

        [ExcludeFromCodeCoverage]
        public PersonStat PersonLevel { get; set; }

        [ExcludeFromCodeCoverage]
        public PerkConditionSubScheme[] Conditions { get; set; }

        [ExcludeFromCodeCoverage]
        public PropSet[] Sources { get; set; }
    }
}
