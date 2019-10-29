namespace Zilon.Core.CommonServices.Dices
{
    internal static class DiceValuesHelper
    {
        /// <summary>
        /// Преобразует выброшенное дробное значение из диапазона [0..1]
        /// в грань n-гранной кости.
        /// </summary>
        /// <param name="rawValue"> Сырое значение в диапазоне [0..1]. </param>
        /// <param name="n"> Количество граней целевой кости. </param>
        /// <returns> Возвращает выбрашенную грань. </returns>
        public static int MapDoubleToDiceEdge(double rawValue, int n)
        {
            return (int)(n * rawValue);
        }
    }
}
