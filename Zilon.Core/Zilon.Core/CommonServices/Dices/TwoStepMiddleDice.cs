using System;

namespace Zilon.Core.CommonServices.Dices
{
    public class TwoStepMiddleDice : IDice
    {
        private const float FORCED_RANGE = 0.3f;
        private const float FORCED_P = 0.9f;
        private readonly Random _random;

        public TwoStepMiddleDice(int seed)
        {
            _random = new Random(seed);
        }

        public int Roll(int n)
        {
            if (n <= 0)
            {
                throw new ArgumentException($"n={n} должно быть положительным ненулевым числом.", nameof(n));
            }

            var forcedDiff = (int)Math.Round(n * FORCED_RANGE);
            var notForcedDiff = n - forcedDiff;

            var forcedLow = (notForcedDiff / 2) + 1;
            var forcedHigh = (forcedLow + forcedDiff) - 1;

            var selectiveRoll = _random.NextDouble();
            if (selectiveRoll <= FORCED_P)
            {
                // Выбираем значения из форсированного диапазона.
                if (forcedHigh - forcedLow > 0)
                {
                    return _random.Next(forcedLow, forcedHigh + 1);
                }

                // В этом диапазоне нет значений.
                // Берём предыдущий диапазон.
                return _random.Next(1, forcedLow + 1);
            }

            // Определяем, из какой нефорсированной половины выбираем числа
            var notForcedSeparator = (1 - FORCED_P) * 0.5f;
            var notForcedSeparatorShiffted = FORCED_P + notForcedSeparator;
            if (selectiveRoll <= notForcedSeparatorShiffted)
            {
                // Берём маленькие числа
                return _random.Next(1, (forcedLow - 1) + 1);
            }

            // Берём большие числа
            return _random.Next(forcedHigh + 1, n + 1);
        }
    }
}