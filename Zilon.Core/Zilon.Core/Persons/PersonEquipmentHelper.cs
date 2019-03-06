using System;

namespace Zilon.Core.Persons
{
    public static class PersonEquipmentHelper
    {
        public static void UnequipProp(this IPerson person, int slotIndex)
        {
            var equipmentCarrier = person.EquipmentCarrier;

            var currentEquipment = equipmentCarrier[slotIndex];

            if (currentEquipment == null)
            {
                throw new InvalidOperationException($"Попытка обнулить слот {slotIndex} без экипировки.");
            }

            equipmentCarrier[slotIndex] = null;
            person.Inventory.Add(currentEquipment);
        }
    }
}
