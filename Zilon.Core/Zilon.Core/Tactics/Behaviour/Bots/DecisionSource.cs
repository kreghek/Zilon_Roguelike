using System.Diagnostics.CodeAnalysis;

using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.Tactics.Behaviour.Bots
{
    public class DecisionSource : IDecisionSource
    {
        private readonly IDice _dice;

        [ExcludeFromCodeCoverage]
        public DecisionSource(IDice dice)
        {
            _dice = dice;
        }

        [ExcludeFromCodeCoverage]
        public int SelectIdleDuration(int min, int max)
        {
            var roll = _dice.Roll(min, max);
            return roll;
        }
    }
}
