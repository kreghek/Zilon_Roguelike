namespace Zilon.Core.Props
{
    /// <summary>
    /// Сервис для работы с прочностью экипировки.
    /// </summary>
    public interface IEquipmentDurableService
    {
        /// <summary>
        /// Обновляет прочность экипировки со временем.
        /// </summary>
        /// <param name="equipment">Целевая экипировка.</param>
        void UpdateByTurn(Equipment equipment);

        /// <summary>
        /// Обновляет прочность экипировки при использовании.
        /// </summary>
        /// <param name="equipment">Целевая экипировка.</param>
        void UpdateByUse(Equipment equipment);

        /// <summary>
        /// Восстановление прочности экипировки.
        /// </summary>
        /// <param name="repairResource">Ресурс, при помощи которого производится ремонт.</param>
        /// <param name="equipment">Целевая экипировка.</param>
        void Repair(IProp repairResource, Equipment equipment);
    }
}
