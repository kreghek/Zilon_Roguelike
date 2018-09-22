using System.Diagnostics.CodeAnalysis;
using Zilon.Core.Components;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Дополнительная схема для хранения правила, которое даёт перк при получении.
    /// </summary>
    public class PerkRuleSubScheme : SubSchemeBase
    {
        /// <summary>
        /// Тип правила.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public PersonRuleType Type { get; set; }

        /// <summary>
        /// Степень влияния правила.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public PersonRuleLevel Level { get; set; }
    }
}
