using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.Persons
{
    public class SurvivalRandomSource : ISurvivalRandomSource
    {
        private readonly IDice _dice;

        public SurvivalRandomSource(IDice dice)
        {
            _dice = dice;
        }

        public int RollMaxHazardDamage()
        {
            return _dice.Roll(6);
        }

        public int RollSurvival(SurvivalStat stat)
        {
            return _dice.Roll(6);
        }
    }
}
