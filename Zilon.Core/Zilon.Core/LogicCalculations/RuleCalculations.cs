using Zilon.Core.Components;

namespace Zilon.Core.LogicCalculations
{
    /// <summary>
    ///     Вспомогательный класс для объединения всех вычислений, связанных с правилами.
    /// </summary>
    public static class RuleCalculations
    {
        /// <summary>
        ///     Расчёт нового модификатора эффективности в зависимости от правила.
        /// </summary>
        /// <param name="currentModifierValue"> Текущие значение модификатора. </param>
        /// <param name="level"> Уровень правила. </param>
        /// <returns> Возвращает новое значение модификатора эффективности. </returns>
        public static int CalcEfficientByRuleLevel(int currentModifierValue, PersonRuleLevel level)
        {
            switch (level)
            {
                case PersonRuleLevel.Lesser:
                    currentModifierValue++;
                    break;

                case PersonRuleLevel.Normal:
                    currentModifierValue += 3;
                    break;

                case PersonRuleLevel.Grand:
                    currentModifierValue += 5;
                    break;

                case PersonRuleLevel.Absolute:
                    currentModifierValue += 10;
                    break;

                default:
                    throw new NotSupportedException("Этот уровень правила не обрабатывается.");
            }

            return currentModifierValue;
        }
    }
}