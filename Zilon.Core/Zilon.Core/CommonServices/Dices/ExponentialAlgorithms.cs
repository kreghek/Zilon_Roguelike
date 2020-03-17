using System;

namespace Zilon.Core.CommonServices.Dices
{
    /// <summary>
    /// Вспомогательный класс для группировки алгоритмов.
    /// </summary>
    public static class ExponentialAlgorithms
    {
        public static double MapToExpo(double u, double lambda)
        {
            var x = Math.Log(1 - u) / -lambda;
            return x;
        }
    }
}
