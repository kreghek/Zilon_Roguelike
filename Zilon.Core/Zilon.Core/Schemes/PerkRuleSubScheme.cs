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
        public PersonRuleType Type { get; set; }

        /// <summary>
        /// Степень влияния правила.
        /// </summary>
        public PersonRuleLevel Level { get; set; }

        /// <summary>
        /// Направление влияния правила.
        /// </summary>
        public PersonRuleDirection Direction { get; set; }

        /// <summary>
        /// Параметры правила.
        /// </summary>
        /// <remarks>
        /// В зависимости от типа правила указывают более точные случае действия правила.
        /// Например, на тип правила Увеличение урона указывает теги предметов, действия которых
        /// будут с увеличением урона.
        /// </remarks>
        public string Params { get; set; }
    }
}
