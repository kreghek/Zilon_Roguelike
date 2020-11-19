using System;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Tactics;

namespace Zilon.Core.MapGenerators
{
    public class ChestGeneratorRandomSource : IChestGeneratorRandomSource
    {
        private const float _startChestProbability = 75f;
        private const float _probabilityDividor = 0.75f;

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
                var successHasChest = 100 - currentProbability;

                if (hasChestRoll >= successHasChest)
                {
                    sum++;
                }
                else
                {
                    break;
                }

                currentProbability = currentProbability * _probabilityDividor;
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

        public PropContainerPurpose RollPurpose()
        {
            var roll = _dice.Roll(100);
            if (roll > 90)
            {
                return PropContainerPurpose.Treasures;
            }

            return PropContainerPurpose.Trash;
        }
    }
}