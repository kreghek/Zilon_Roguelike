using System;

namespace Zilon.Core.Persons
{
    internal class EquipmentCarrier : IEquipmentCarrier
    {
        public EquipmentCarrier(int slotCount)
        {
            Equipments = new Equipment[slotCount];
        }

        public Equipment[] Equipments { get; private set; }

        public event EventHandler<EventArgs> EquipmentChanged;


        public void SetEquipment(Equipment equipment, int slotIndex)
        {
            var oldEquipment = Equipments[slotIndex];
            Equipments[slotIndex] = equipment;

            DoEquipmentChanged(slotIndex, oldEquipment, equipment);
        }

        protected void DoEquipmentChanged(int slotIndex,
            Equipment oldEquipment,
            Equipment equipment)
        {
            EquipmentChanged?.Invoke(this, new EventArgs());
        }
    }
}
