using Newtonsoft.Json;

using Zilon.Core.Components;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Общее правило, срабатывающее при использовании предмета.
    /// </summary>
    /// <remarks>
    /// Сейчас используется при:
    /// - Поглощении провинта.
    /// - Поглощении медикаментов.
    /// </remarks>
    public sealed class ConsumeCommonRule
    {
        /// <summary>
        /// Конструирует объект правила.
        /// </summary>
        /// <param name="type">Тип правила.</param>
        /// <param name="level">Уровень влияния правила.</param>
        /// <param name="direction">Направление влияния (бонус/штраф).</param>
        [JsonConstructor]
        public ConsumeCommonRule(
            ConsumeCommonRuleType type,
            PersonRuleLevel level,
            PersonRuleDirection direction = PersonRuleDirection.Positive)
        {
            Type = type;
            Level = level;
            Direction = direction;

            if (Direction == 0)
            {
                Direction = PersonRuleDirection.Positive;
            }
        }

        /// <summary>Направление влияния (бонус/штраф).</summary>
        public PersonRuleDirection Direction { get; }

        /// <summary>Уровень влияния правила.</summary>
        public PersonRuleLevel Level { get; }

        /// <summary>Тип правила.</summary>
        public ConsumeCommonRuleType Type { get; }
    }
}