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
        /// Рассчитывает успешный бросок для прохода обороны.
        /// </summary>
        /// <param name="defenceItem"> Проверяемая оборона. </param>
        /// <returns> Возвращает число, указывающее минимальный бросок на пробитие обороны. </returns>
        public static int CalcSuccessToHit([CanBeNull] PersonDefenceItem defenceItem)
        {

            // При броске в 1 - неудачная нападение, даже если нет обороны.
            // Приравнивается к обороне уровня None
            if (defenceItem == null)
            {
                return 2;
            }

            var successToHit = CalcSuccessToHitRollInner(defenceItem.Level);

            return successToHit;
        }

        /// <summary>
        /// Возвращает оборону с наиболее предпочтительными характеристиками. Фактически, самого высокого уровня.
        /// </summary>
        /// <param name="currentDefences"> Текущие обороны. </param>
        /// <returns> Возвращает объект предпочтительной обороны. </returns>
        public static PersonDefenceItem CalcPreferredDefense(IEnumerable<PersonDefenceItem> currentDefences)
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
            switch (level)
            {
                case PersonRuleLevel.None:
                    return 2;

                case PersonRuleLevel.Lesser:
                    return 4;

                case PersonRuleLevel.Normal:
                    return 5;

                case PersonRuleLevel.Grand:
                    return 6;

                case PersonRuleLevel.Absolute:
                    return 8;

                default:
                    throw new ArgumentException($"Неизвестное значение {level}.", nameof(level));
            }
        }
    }
}