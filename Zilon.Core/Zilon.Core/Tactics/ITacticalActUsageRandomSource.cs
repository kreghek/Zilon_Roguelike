using Zilon.Core.Common;
using Zilon.Core.Props;

namespace Zilon.Core.Tactics
{
    /// <summary>
    ///     Источник случайных значений для совершаемых действий.
    /// </summary>
    public interface ITacticalActUsageRandomSource
    {
        /// <summary>
        ///     Выбирает значение эффективности действия по указанным характеристикам броска.
        /// </summary>
        /// <param name="roll"> Характеристики броска. </param>
        /// <returns> Возвращает случайное значение эффективности использования. </returns>
        int RollEfficient(Roll roll);

        /// <summary>
        ///     Бросок проверки на попадание действием.
        /// </summary>
        /// <param name="roll"> Характеристики броска. </param>
        /// <returns> Возвращает результат броска D6. </returns>
        int RollToHit(Roll roll);

        /// <summary>
        ///     Бросок проверки на защиту бронёй.
        /// </summary>
        /// <returns> Возвращает результат броска D6. </returns>
        int RollArmorSave();

        /// <summary>
        ///     Бросок проверки на использование дополнительных действий.
        /// </summary>
        /// <returns> Возвращает результат броска D6. </returns>
        /// <remarks>
        ///     Используется для проверки удара вторым оружием.
        /// </remarks>
        int RollUseSecondaryAct();

        /// <summary>
        ///     Выбирает среди надетых предметов случайный предмет,
        ///     который был повреждён в результате действия.
        /// </summary>
        /// <param name="armorEquipments">Доступные предметы экипировки.</param>
        /// <returns> Случайный экипированный предмет, который был повреждён. </returns>
        Equipment RollDamagedEquipment(IEnumerable<Equipment> armorEquipments);
    }
}