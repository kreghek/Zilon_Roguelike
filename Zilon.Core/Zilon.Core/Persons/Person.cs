using System;
using System.Collections.Generic;

using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    public class Person : IPerson
    {
        public int Id { get; set; }

        public string Name { get; }

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

        private void EquipmentCarrier_EquipmentChanged(object sender, EventArgs e)
        {
            TacticalActCarrier.Acts = CalcActs(EquipmentCarrier.Equipments);
        }

        private static ITacticalAct[] CalcActs(IEnumerable<Equipment> equipments)
        {
            if (equipments == null)
            {
                throw new ArgumentNullException(nameof(equipments));
            }

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

        public override string ToString()
        {
            return $"{Name}";
        }

    }
}
