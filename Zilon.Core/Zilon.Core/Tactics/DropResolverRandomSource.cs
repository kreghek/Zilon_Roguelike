using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.Tactics
{
    public class DropResolverRandomSource : IDropResolverRandomSource
    {
        private readonly IDice _dice;

        public DropResolverRandomSource(IDice dice)
        {
            _dice = dice;
        }

        public int RollEquipmentPower(int minPower, int maxPower)
        {
            return Roll(minPower, maxPower);
        }

        public int RollResourceCount(int minCount, int maxCount)
        {
            return Roll(minCount, maxCount);
        }

        public int RollWeight(int totalWeight)
        {
            var roll = _dice.Roll(totalWeight);
            return roll;
        }

        //TODO Вынести в расширения для кости
        private int Roll(int min, int max)
        {
            //TODO Добавить проверки: min <= max, не нули или обработать нули
            var range = max - min;
            var roll = _dice.Roll(range);
            return roll + min;
        }
    }
}
