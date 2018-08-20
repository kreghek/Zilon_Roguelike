using System;

namespace Zilon.Core.Persons
{
    public class EquipmentCarrier : IEquipmentCarrier
    {
        public EquipmentCarrier(int slotCount)
        {
            Equipments = new Equipment[slotCount];
        }

        public Equipment[] Equipments { get; }

        public event EventHandler<EquipmentChangedEventArgs> EquipmentChanged;


        public void SetEquipment(Equipment equipment, int slotIndex)
        {
            var oldEquipment = Equipments[slotIndex];
            Equipments[slotIndex] = equipment;

            DoEquipmentChanged(slotIndex, oldEquipment, equipment);
        }

        private void DoEquipmentChanged(int slotIndex,
            Equipment oldEquipment,
            Equipment equipment)
        {
            EquipmentChanged?.Invoke(this, new EquipmentChangedEventArgs(equipment, oldEquipment, slotIndex));
        }
    }
}
