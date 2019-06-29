using Zilon.Core.Persons;

namespace Zilon.Core.Tactics
{
    public class UseActResolver : IUseActResolver
    {
        private readonly ITacticalActUsageRandomSource _actUsageRandomSource;

        public UseActResolver(ITacticalActUsageRandomSource actUsageRandomSource)
        {
            _actUsageRandomSource = actUsageRandomSource;
        }

        public bool SecondaryActUsePass(Actor actor, ITacticalAct act)
        {
            var useSuccessRoll = GetUseSuccessRoll();
            var useFactRoll = GetUseFactRoll();

            if (useFactRoll < useSuccessRoll)
            {
                return false;
            }

            return true;
        }


        private int GetUseFactRoll()
        {
            var roll = _actUsageRandomSource.RollUseSecondaryAct();
            return roll;
        }

        private int GetUseSuccessRoll()
        {
            // В будущем успех использования вторичных дейсвий будет зависить от действия, экипировки, перков.
            return 5;
        }
    }
}
