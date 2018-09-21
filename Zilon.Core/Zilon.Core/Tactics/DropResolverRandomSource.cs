using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.Tactics
{
    public class DropResolverRandomSource : IDropResolverRandomSource
    {
        private readonly IDice _dice;

        public DropResolverRandomSource(IDice dice)
        {
            _dice = dice;
        }

        public int RollEquipmentPower(int minPower, int maxPower)
        {
            return _dice.Roll(minPower, maxPower);
        }

        public int RollResourceCount(int minCount, int maxCount)
        {
            return _dice.Roll(minCount, maxCount);
        }

        public int RollWeight(int totalWeight)
        {
            var roll = _dice.Roll(totalWeight);
            return roll;
        }
    }
}
