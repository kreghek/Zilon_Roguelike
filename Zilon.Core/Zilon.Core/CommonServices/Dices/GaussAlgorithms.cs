using System;

namespace Zilon.Core.CommonServices.Dices
{
    /// <summary>
    /// Вспомогательный класс для группировки алгоритмов.
    /// </summary>
    public static class GaussAlgorithms
    {
        public static double GetRandomStdNormal(double u1, double u2)
        {
            var randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2);
            return randStdNormal;
        }
    }
}
