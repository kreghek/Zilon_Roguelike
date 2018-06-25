using System;

using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour.Bots
{
    using Zilon.Core.CommonServices.Dices;

    public class DecisionSource : IDecisionSource
    {
        private readonly IDice _dice;

        public DecisionSource(IDice dice) {
            _dice = dice;
        }

        public int SelectIdleDuration(int min, int max)
        {
            var roll = _dice.Roll(max) + min - 1;
            return roll;
        }

        public IMapNode SelectPatrolPoint(IMap map, IPatrolRoute patrolRoute)
        {
            throw new NotImplementedException();
        }
    }
}
