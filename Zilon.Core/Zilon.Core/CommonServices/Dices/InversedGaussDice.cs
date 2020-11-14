namespace Zilon.Core.CommonServices.Dices
{
    /// <summary>
    ///     Генератор случайных числе Парка-Миллера.
    /// </summary>
    public sealed class InversedGaussDice : GaussDice
    {
        [ExcludeFromCodeCoverage]
        public InversedGaussDice()
        {
        }

        [ExcludeFromCodeCoverage]
        public InversedGaussDice(int seed) : base(seed)
        {
        }

        protected override double ProcessRandStdNormal(double randStdNormal, double mean)
        {
            double inversedRandStdNormal;
            if (randStdNormal >= 0)
            {
                inversedRandStdNormal = mean - randStdNormal;
            }
            else
            {
                inversedRandStdNormal = -(mean + randStdNormal);
            }

            return inversedRandStdNormal;
        }
    }
}