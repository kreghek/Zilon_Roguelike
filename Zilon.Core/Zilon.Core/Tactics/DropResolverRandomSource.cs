using System.Diagnostics.CodeAnalysis;

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
        public int RollEquipmentPower(int minPower, int maxPower)
        {
            return _dice.Roll(minPower, maxPower);
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
