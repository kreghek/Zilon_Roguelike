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
            var roll = _dice.Roll(6);
            return roll;
        }

        public float RollEfficient(Roll roll)
        {
            var sum = 0;
            for (var i = 0; i < roll.Count; i++)
            {
                var currentRoll = _dice.Roll(roll.Dice);
                sum += currentRoll;
            }

            return sum;
        }
    }
}
