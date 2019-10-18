namespace Zilon.Core.CommonServices
{
    /// <summary>
    /// Интерфейс генератора случайных чисел.
    /// </summary>
    public interface IRandomNumberGenerator
    {
        /// <summary>
        /// Возвращает последовательность случайных числе.
        /// </summary>
        /// <param name="count"> Количество элементов последовательности. </param>
        /// <returns>Возвращает набор случайных числе.</returns>
        double[] GetSequence(int count);
    }
}