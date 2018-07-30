using Zilon.Core.Components;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Дополнительная схема для хранения правила, которое даёт перк при получении.
    /// </summary>
    public class PerkRuleSubScheme: SubSchemeBase
    {
        /// <summary>
        /// Тип правила.
        /// </summary>
        public PersonRuleType Type { get; set; }

        /// <summary>
        /// Степень влияния правила.
        /// </summary>
        public PersonRuleLevel Level { get; set; }
    }
}
