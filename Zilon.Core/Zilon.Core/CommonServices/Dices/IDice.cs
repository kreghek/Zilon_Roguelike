namespace Zilon.Core.CommonServices.Dices
{
    /// <summary>
    /// Интерфейс кости.
    /// </summary>
    public interface IDice
    {
        /// <summary>
        /// Возвращает результат броска n-гранной кости.
        /// </summary>
        /// <param name="n"> Количество граней кости. </param>
        /// <returns> Результат броска. </returns>
        int Roll(int n);
    }
}
