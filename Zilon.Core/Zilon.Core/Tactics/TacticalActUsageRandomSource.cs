using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Zilon.Core.Common;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Props;

namespace Zilon.Core.Tactics
{
    public class TacticalActUsageRandomSource : ITacticalActUsageRandomSource
    {
        private readonly IDice _dice;

        [ExcludeFromCodeCoverage]
        public TacticalActUsageRandomSource(IDice dice)
        {
            _dice = dice;
        }

        /// <summary>Бросок проверки на попадание действием.</summary>
        /// <returns>Возвращает результат броска D6.</returns>
        public int RollToHit(Roll roll)
        {
            return RollWithModifiers(roll);
        }

        /// <summary>
        /// Выбирает значение эффективности действия по указанным характеристикам броска.
        /// </summary>
        /// <param name="roll">Характеристики броска.</param>
        /// <returns>Возвращает случайное значение эффективности использования.</returns>
        public int RollEfficient(Roll roll)
        {
            return RollWithModifiers(roll);
        }

        private int RollWithModifiers(Roll roll)
        {
            var sum = 0;
            for (var i = 0; i < roll.Count; i++)
            {
                var currentRoll = _dice.Roll(roll.Dice);

                if (roll.Modifiers != null)
                {
                    currentRoll += roll.Modifiers.ResultBuff;
                }

                if (currentRoll <= 0)
                {
                    currentRoll = 1;
                }

                sum += currentRoll;
            }

            return sum;
        }

        /// <summary>Бросок проверки на защиту бронёй.</summary>
        /// <returns>Возвращает результат броска D6.</returns>
        public int RollArmorSave()
        {
            return RollD6();
        }

        /// <summary>Бросок проверки на использование дополнительных действий.</summary>
        /// <returns>Возвращает результат броска D6.</returns>
        /// <remarks>Используется для проверки удара вторым оружием.</remarks>
        public int RollUseSecondaryAct()
        {
            return RollD6();
        }

        /// <summary>
        /// Выбирает среди надетых предметов случайный предмет,
        /// который был повреждён в результате действия.
        /// </summary>
        /// <param name="armorEquipments">Доступные предметы экипировки.</param>
        /// <returns>Случайный экипированный предмет, который был повреждён.</returns>
        public Equipment RollDamagedEquipment(IEnumerable<Equipment> armorEquipments)
        {
            var count = armorEquipments.Count();
            if (count == 0)
            {
                return null;
            }

            var rollIndex = _dice.Roll(0, count - 1);
            return armorEquipments.ElementAt(rollIndex);
        }


        private int RollD6()
        {
            var roll = _dice.Roll(6);
            return roll;
        }
    }
}