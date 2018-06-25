using System;

namespace Zilon.Core.CommonServices.Dices
{
    public class Dice : IDice
    {
        private readonly Random _random = new Random();

        public int Roll(int n)
        {
            var rollResult = _random.Next(1, n + 1);
            return rollResult;
        }
    }
}
