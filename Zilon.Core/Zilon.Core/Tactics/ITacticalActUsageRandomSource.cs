using Zilon.Core.Common;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Источник случайных значений для совершаемых действий.
    /// </summary>
    public interface ITacticalActUsageRandomSource
    {
        /// <summary>
        /// Выбирает значение эффективности действия по указанным характеристикам броска.
        /// </summary>
        /// <param name="roll"> Характеристики броска. </param>
        /// <returns> Возвращает случайное значение эффективности использования. </returns>
        int RollEfficient(Roll roll);

        int RollToHit();
    }
}