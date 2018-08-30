using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.Tactics.Behaviour.Bots
{
    public class DecisionSource : IDecisionSource
    {
        private const int FLOAT_MULTY = 10;
        private readonly IDice _dice;

        public DecisionSource(IDice dice)
        {
            _dice = dice;
        }

        public int SelectIdleDuration(int min, int max)
        {
            var roll = _dice.Roll(max) + min - 1;
            return roll;
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
