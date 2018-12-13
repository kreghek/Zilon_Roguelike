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

        /// <summary>
        /// Бросок проверки на попадание действием.
        /// </summary>
        /// <returns> Возвращает результат броска D6. </returns>
        int RollToHit();

        /// <summary>
        /// Бросок проверки на защиту бронёй.
        /// </summary>
        /// <returns> Возвращает результат броска D6. </returns>
        int RollArmorSave();

        /// <summary>
        /// Бросок проверки на использование дополнительных действий.
        /// </summary>
        /// <returns> Возвращает результат броска D6. </returns>
        /// <remarks>
        /// Используется для проверки удара вторым оружием.
        /// </remarks>
        int RollUseSecondaryAct();
    }
}