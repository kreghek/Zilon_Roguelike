using System.Collections.Generic;

namespace Zilon.Core.Persons
{
    public class Person : IPerson, ITacticalActCarrier
    {
        const int SLOT_COUNT = 3;

        public int Id { get; set; }

        public float Hp { get; set; }

        public IEquipmentCarrier EquipmentCarrier { get; }

        public ITacticalAct[] Acts { get; set; }

        public Person()
        {
            EquipmentCarrier = new EquipmentCarrier(SLOT_COUNT);
            EquipmentCarrier.EquipmentChanged += EquipmentCarrier_EquipmentChanged;
        }

        private void EquipmentCarrier_EquipmentChanged(object sender, System.EventArgs e)
        {
            Acts = CalcActs(EquipmentCarrier.Equipments);
        }

        private static ITacticalAct[] CalcActs(IEnumerable<Equipment> equipments)
        {
            var actList = new List<ITacticalAct>();

            foreach (var equipment in equipments)
            {
                actList.AddRange(equipment.Acts);
            }

            return actList.ToArray();
        }

    }
}
