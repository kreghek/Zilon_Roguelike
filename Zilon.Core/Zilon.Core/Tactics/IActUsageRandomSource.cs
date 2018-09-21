namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Источник случайных значений для совершаемых действий.
    /// </summary>
    public interface IActUsageRandomSource
    {
        /// <summary>
        /// Выбирает значение эффективности действия в указанном диапазоне.
        /// </summary>
        /// <param name="minEfficient"></param>
        /// <param name="maxEfficient"></param>
        /// <returns> Возвращает случайное значение эффективности использования
        /// действия в указанном диапазоне эффективности. </returns>
        float SelectEfficient(float minEfficient, float maxEfficient);
    }
}