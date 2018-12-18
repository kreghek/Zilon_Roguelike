using System;
using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.MapGenerators
{
    public class ChestGeneratorRandomSource : IChestGeneratorRandomSource
    {
        private const int _startChestProbability = 50;
        private const int _probabilityDividor = 2;

        private readonly IDice _dice;

        public ChestGeneratorRandomSource(IDice dice)
        {
            _dice = dice;
        }

        public int RollChestCount(int maxCount)
        {
            if (maxCount <= 0)
            {
                throw new ArgumentException($"Значение {maxCount} не может быть меньше или равно 0.", nameof(maxCount));
            }

            var currentProbability = _startChestProbability;

            var sum = 0;
            for (var i = 0; i < maxCount; i++)
            {
                var hasChestRoll = _dice.Roll(100);

                if (hasChestRoll >= currentProbability)
                {
                    sum++;
                }
                else
                {
                    break;
                }

                currentProbability = currentProbability / _probabilityDividor;
                if (currentProbability <= 0)
                {
                    break;
                }
            }

            return sum;
        }

        public int RollNodeIndex(int nodeCount)
        {
            var rolledIndex = _dice.Roll(0, nodeCount - 1);
            return rolledIndex;
        }
    }
}
