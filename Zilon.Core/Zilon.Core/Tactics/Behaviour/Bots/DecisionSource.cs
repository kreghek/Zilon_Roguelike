using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.Tactics.Behaviour.Bots
{
    public class DecisionSource : IDecisionSource
    {
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
            throw new System.NotImplementedException();
        }
    }
}
