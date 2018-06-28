namespace Zilon.Core.Tactics.Behaviour.Bots
{
    /// <summary>
    /// Источник решений для AI.
    /// </summary>
    public interface IDecisionSource
    {
        /// <summary>
        /// Выбирает длительность ожидания в ходах.
        /// </summary>
        /// <param name="min"> Минимальное количество ходов ожидания. </param>
        /// <param name="max"> Максимальное количество ходов ожидания. </param>
        /// <returns> Количество ходов ожидания. </returns>
        int SelectIdleDuration(int min, int max);

        /// <summary>
        /// Выбирает значение эффективности действия.
        /// </summary>
        /// <param name="minEfficient"></param>
        /// <param name="maxEfficient"></param>
        /// <returns></returns>
        float SelectEfficient(float minEfficient, float maxEfficient);
    }
}
