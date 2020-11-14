using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.Tactics
{
    public class DropResolverRandomSource : IDropResolverRandomSource
    {
        private readonly IDice _dice;

        [ExcludeFromCodeCoverage]
        public DropResolverRandomSource(IDice dice)
        {
            _dice = dice;
        }

        [ExcludeFromCodeCoverage]
        public int RollResourceCount(int minCount, int maxCount)
        {
            return _dice.Roll(minCount, maxCount);
        }

        [ExcludeFromCodeCoverage]
        public int RollWeight(int totalWeight)
        {
            var roll = _dice.Roll(totalWeight);
            return roll;
        }
    }
}