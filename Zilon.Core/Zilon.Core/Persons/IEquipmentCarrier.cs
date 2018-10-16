using System;

using Zilon.Core.Props;
using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Интерфейс для работы с экипировкой.
    /// </summary>
    public interface IEquipmentCarrier
    {
        /// <summary>
        /// Экипировка персонажа.
        /// </summary>
        Equipment[] Equipments { get; }

        PersonSlotSubScheme[] Slots { get; }

        /// <summary>
        /// Устанавливает экипировку.
        /// </summary>
        /// <param name="equipment">
        /// Экипировка.
        /// Если указано null, то экипировка изымается из указанного слота.
        /// </param>
        /// <param name="slotIndex"> Индекс слота экипировки. </param>
        void SetEquipment(Equipment equipment, int slotIndex);

        /// <summary>
        /// Выстреливает, когда экипировка изменяется.
        /// </summary>
        event EventHandler<EquipmentChangedEventArgs> EquipmentChanged;
    }
}
