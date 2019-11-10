namespace Zilon.Core.CommonServices.Dices
{
    /// <summary>
    /// Генератор случайных числе Парка-Миллера.
    /// </summary>
    public sealed class InversedGaussDice : DiceBase, IDice
    {
        /// <summary>
        /// Погрешность.
        /// </summary>
        private const double STDDEV = 0.25;

        /// <summary>
        /// Математическое ожидание.
        /// </summary>
        private const double MEAN = 0.5;

        public int Roll(int n)
        {
            var rand = GetNext(0.0, 1.0);
            var roll = DiceValuesHelper.MapDoubleToDiceEdge(rand, n);
            var inversedRoll = n - roll + 1;
            return inversedRoll;
        }

        private double GetNext(double min, double max)
        {
            double x;
            do
            {
                x = NextGaussian(MEAN, STDDEV);
            } while (x < min || x > max);
            return x;
        }

        private double NextGaussian(double mean, double stdDev)
        {
            var u1 = GetNextDouble();
            var u2 = GetNextDouble();
            var randStdNormal = GaussAlgorithms.GetRandomStdNormal(u1, u2);

            var randNormal = stdDev * randStdNormal;

            double inversedRandStdNormal;
            if (randNormal >= 0)
            {
                inversedRandStdNormal = mean - randNormal;
            }
            else
            {
                inversedRandStdNormal = -(mean + randNormal);
            }

            var randTotal = mean + inversedRandStdNormal;

            return randTotal;
        }
    }
}
