using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.MapGenerators
{
    public class ChestGeneratorRandomSource : IChestGeneratorRandomSource
    {
        private readonly IDice _dice;

        public ChestGeneratorRandomSource(IDice dice)
        {
            _dice = dice;
        }

        public int RollChestCount(int maxCount)
        {
            var hasChestRoll = _dice.Roll(100);
            if (100-hasChestRoll <= 15)
            {
                var chestCount = _dice.Roll(1, maxCount);
                return chestCount;
            }
            {
                return 0;
            }
        }

        public int RollNodeIndex(int nodeCount)
        {
            var rolledIndex = _dice.Roll(0, nodeCount - 1);
            return rolledIndex;
        }
    }
}
