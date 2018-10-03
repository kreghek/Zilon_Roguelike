using System.Diagnostics.CodeAnalysis;

using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.Tactics
{
    public class TacticalActUsageRandomSource : ITacticalActUsageRandomSource
    {
        private const int FLOAT_MULTY = 10;

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

        public float SelectEfficient(float minEfficient, float maxEfficient)
        {
            var min = (int)(minEfficient * FLOAT_MULTY);
            var max = (int)(maxEfficient * FLOAT_MULTY);

            var roll = _dice.Roll(min, max);
            var rollFloat = (float)roll; // иначе будет деление int и пропадут дроби.

            return rollFloat / FLOAT_MULTY;
        }
    }
}
