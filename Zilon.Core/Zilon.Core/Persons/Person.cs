using System.Collections.Generic;

namespace Zilon.Core.Persons
{
    using System;
    using Zilon.Core.Schemes;

    public class Person : IPerson
    {
        public int Id { get; set; }

        public float Hp => Scheme.Hp;

        public IEquipmentCarrier EquipmentCarrier { get; }

        public ITacticalActCarrier TacticalActCarrier { get; }

        public PersonScheme Scheme { get; }

        public Person(PersonScheme scheme)
        {
            if (scheme == null)
            {
                throw new ArgumentNullException(nameof(scheme));
            }

            Scheme = scheme;

            var slotCount = Scheme.SlotCount;
            EquipmentCarrier = new EquipmentCarrier(slotCount);
            EquipmentCarrier.EquipmentChanged += EquipmentCarrier_EquipmentChanged;

            TacticalActCarrier = new TacticalActCarrier();
        }

        private void EquipmentCarrier_EquipmentChanged(object sender, System.EventArgs e)
        {
            TacticalActCarrier.Acts = CalcActs(EquipmentCarrier.Equipments);
        }

        private static ITacticalAct[] CalcActs(IEnumerable<Equipment> equipments)
        {
            var actList = new List<ITacticalAct>();

            foreach (var equipment in equipments)
            {
                if (equipment == null)
                {
                    continue;
                }

                actList.AddRange(equipment.Acts);
            }

            return actList.ToArray();
        }

    }
}
