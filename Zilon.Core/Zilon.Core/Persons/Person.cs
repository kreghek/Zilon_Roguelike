using System.Collections.Generic;
using System.Linq;

namespace Zilon.Core.Persons
{
    public class Person : IPerson, ITacticalActCarrier
    {
        public int Id { get; set; }

        public float Hp { get; set; }

        public IEquipmentCarrier EquipmentCarrier { get; }

        public ITacticalAct DefaultAct { get; set; }

        public ITacticalAct[] Acts { get; set; }

        public Person()
        {
            EquipmentCarrier = new EquipmentCarrier();
            EquipmentCarrier.EquipmentChanged += EquipmentCarrier_EquipmentChanged;
        }

        private void EquipmentCarrier_EquipmentChanged(object sender, System.EventArgs e)
        {
            Acts = CalcActs(Equipments);
            DefaultAct = Acts.FirstOrDefault();
        }

        private static ITacticalAct[] CalcActs(IEnumerable<Equipment> equipments)
        {
            var actList = new List<ITacticalAct>();

            foreach (var equipment in equipments)
            {
                var actSchemeSids = equipment.Scheme.Equip.ActSids;

                foreach (var actSchemeSid in actSchemeSids)
                {

                    var act = new TacticalAct(actScheme);
                    actList.Add(act);
                }
            }

            return actList.ToArray();
        }
    }
}
