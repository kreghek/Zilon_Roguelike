using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zilon.Core.CommonServices
{
    public class RandomNumberGenerator
    {
        uint init_seed;  // Initial random seed value
        uint cur_seed;   // Current random seed value
        uint num_draws;  // Dimensionality of the RNG

        public virtual uint get_random_seed() => cur_seed;


        public void set_random_seed(uint _seed) => cur_seed = _seed;

        public void reset_random_seed() => cur_seed = init_seed;

        public void set_num_draws(uint _num_draws) => num_draws = _num_draws;

        /// <summary>
        /// Obtain a random integer (needed for creating random uniforms)
        /// </summary>
        public virtual uint get_random_integer() => 0;

        /// <summary>
        /// Fills a vector with uniform random variables on the open interval (0,1)
        /// </summary>
        /// <param name="draws"></param>
        public virtual void get_uniform_draws(double[] draws) => 0;
    }

    public class LinearCongruentialGenerator : RandomNumberGenerator
    {
        private int max_multiplier;


    }
}
