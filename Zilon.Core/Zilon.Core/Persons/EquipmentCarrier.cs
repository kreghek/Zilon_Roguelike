using System;

namespace Zilon.Core.Persons
{
    class EquipmentCarrier : IEquipmentCarrier
    {
        public Equipment[] Equipments => throw new NotImplementedException();

        public event EventHandler<EventArgs> EquipmentChanged;

        public void SetEquipment(Equipment equipment, int slotIndex)
        {
            throw new NotImplementedException();
        }
    }
}
