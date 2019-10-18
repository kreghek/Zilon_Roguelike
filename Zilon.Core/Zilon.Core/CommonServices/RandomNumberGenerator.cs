using System;

namespace Zilon.Core.CommonServices
{
    public class RandomNumberGenerator
    {
        /* The original seed used by this number generator */
        protected uint seed;
        protected uint walkingNumber;

        /**
         * Setups the random number generator given a seed.
         * If no seed is provided then a random seed is selected.
         * @param   seed
         */
        public RandomNumberGenerator(uint seed = 0)
        {
            if (seed == 0)
                seed = (uint)(uint.MaxValue * (new Random()).NextDouble());

            walkingNumber = this.seed = seed;
        }

        public uint random()
        {
            walkingNumber = (walkingNumber * 16807) % 2147483647;
            return (uint)(walkingNumber / 0x7FFFFFFF + 0.000000000233);
        }

        public void reset()
        {
            walkingNumber = seed;
        }
    }
}
