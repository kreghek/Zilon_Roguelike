using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Common;
using Zilon.Core.CommonServices.Dices;

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

        public int RollToHit()
        {
            return RollD6();
        }

        public int RollEfficient(Roll roll)
        {
            var sum = 0;
            for (var i = 0; i < roll.Count; i++)
            {
                var currentRoll = _dice.Roll(roll.Dice);
                sum += currentRoll;
            }

            return sum;
        }

        public int RollArmorSave()
        {
            return RollD6();
        }

        private int RollD6()
        {
            var roll = _dice.Roll(6);
            return roll;
        }
    }
}
