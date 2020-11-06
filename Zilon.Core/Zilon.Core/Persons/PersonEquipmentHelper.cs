using System;

using Zilon.Core.PersonModules;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Вспомогательный класс для работы с экипировкой персонажа.
    /// </summary>
    public static class PersonEquipmentHelper
    {
        /// <summary>Деэкипировка предмета.</summary>
        /// <param name="person">Пресонаж, у которого снимается предмет.</param>
        /// <param name="slotIndex">Индекс слота, из которого снимается предмет.</param>
        /// <exception cref="System.InvalidOperationException">
        /// Происходит, если обнаружена попытка обнулить слот {slotIndex},
        /// в котором нет экипировки.
        /// </exception>
        public static void UnequipProp(this IPerson person, int slotIndex)
        {
            if (person is null)
            {
                throw new ArgumentNullException(nameof(person));
            }

            var equipmentCarrier = person.GetModule<IEquipmentModule>();

            var currentEquipment = equipmentCarrier[slotIndex];

            if (currentEquipment == null)
            {
                throw new InvalidOperationException($"Попытка обнулить слот {slotIndex} без экипировки.");
            }

            equipmentCarrier[slotIndex] = null;
            person.GetModule<IInventoryModule>().Add(currentEquipment);
        }
    }
}