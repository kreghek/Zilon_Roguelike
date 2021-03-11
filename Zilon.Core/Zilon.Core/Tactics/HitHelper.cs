using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using Zilon.Core.Components;
using Zilon.Core.Persons;

namespace Zilon.Core.Tactics
{
    public static class HitHelper
    {
        /// <summary>
        /// Возвращает оборону с наиболее предпочтительными характеристиками. Фактически, самого высокого уровня.
        /// </summary>
        /// <param name="currentDefences"> Текущие обороны. </param>
        /// <returns> Возвращает объект предпочтительной обороны. </returns>
        public static PersonDefenceItem? CalcPreferredDefense(IEnumerable<PersonDefenceItem> currentDefences)
        {
            var currentDefensesArray = currentDefences.ToArray();
            if (!currentDefensesArray.Any())
            {
                return null;
            }

            var sortedDefenses = currentDefensesArray.OrderByDescending(x => x.Level);
            var preferredDeference = sortedDefenses.First();
            return preferredDeference;
        }

        /// <summary>
        /// Рассчитывает успешный бросок для прохода обороны.
        /// </summary>
        /// <param name="defenceItem"> Проверяемая оборона. </param>
        /// <returns> Возвращает число, указывающее минимальный бросок на пробитие обороны. </returns>
        public static int CalcSuccessToHit([CanBeNull] PersonDefenceItem? defenceItem)
        {
            // При броске в 1 - неудачная нападение, даже если нет обороны.
            // Приравнивается к обороне уровня None
            if (defenceItem is null)
            {
                return 2;
            }

            var successToHit = CalcSuccessToHitRollInner(defenceItem.Level);

            return successToHit;
        }

        /// <summary>
        /// Возвращает тип обороны, которая может быть использована для отражения указанного наступления.
        /// </summary>
        /// <param name="offenceType"> Тип наступления. </param>
        /// <returns> Возвращает экземпляр типа обороны. </returns>
        public static DefenceType GetDefence(OffenseType offenceType)
        {
            var rawValue = (int)offenceType;
            var defenceType = (DefenceType)rawValue;
            return defenceType;
        }

        /// <summary>
        /// Рассчитывает минимальное значение броска D6, необходимого для пробития указанной обороны.
        /// </summary>
        /// <param name="level"> Уровень обороны, для которой вычисляется нижный порог броска D6. </param>
        /// <returns> Минимальный погод броска D6. </returns>
        private static int CalcSuccessToHitRollInner(PersonRuleLevel level)
        {
            return level switch
            {
                PersonRuleLevel.None => 2,
                PersonRuleLevel.Lesser => 4,
                PersonRuleLevel.Normal => 5,
                PersonRuleLevel.Grand => 6,
                PersonRuleLevel.Absolute => 8,
                _ => throw new ArgumentException($"Неизвестное значение {level}.", nameof(level)),
            };
        }
    }
}