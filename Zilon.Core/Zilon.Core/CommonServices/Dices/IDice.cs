namespace Zilon.Core.CommonServices.Dices
{
    /// <summary>
    /// Интерфейс кости.
    /// </summary>
    public interface IDice
    {
        /// <summary>
        /// Возвращает результат броска n-гранной кости. Минимальное значение будет 1.
        /// </summary>
        /// <param name="n"> Количество граней кости. </param>
        /// <returns> Результат броска. </returns>
        int Roll(int n);
    }
}
