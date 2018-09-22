using System.Diagnostics.CodeAnalysis;

using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.Tactics
{
    public class ActUsageRandomSource : IActUsageRandomSource
    {
        private const int FLOAT_MULTY = 10;

        private readonly IDice _dice;

        [ExcludeFromCodeCoverage]
        public ActUsageRandomSource(IDice dice)
        {
            _dice = dice;
        }

        public float SelectEfficient(float minEfficient, float maxEfficient)
        {
            var min = (int)(minEfficient * FLOAT_MULTY);
            var max = (int)(maxEfficient * FLOAT_MULTY);

            var roll = _dice.Roll(max) + min - 1;

            return (float)roll / FLOAT_MULTY;
        }
    }
}
