using System.Collections.Generic;
using System.Linq;

namespace Zilon.Core.Persons
{
    public class Person : IPerson, IEquipCarrier, ITacticalActCarrier
    {
        public int Id { get; set; }

        public float Hp { get; set; }

        public Equipment[] Equipments { get; }

        public ITacticalAct DefaultAct { get; }

        public ITacticalAct[] Acts { get; }

        public Person(Equipment equipment)
        {
            if (equipment != null)
            {
                Equipments = new[] { equipment };
                Acts = CalcActs(Equipments);
                DefaultAct = Acts.FirstOrDefault();
            }
        }

        private static ITacticalAct[] CalcActs(IEnumerable<Equipment> equipments)
        {
            var actList = new List<ITacticalAct>();

            foreach (var equipment in equipments)
            {
                var actSchemes = equipment.Scheme.Equip.Acts;

                foreach (var actScheme in actSchemes)
                {
                    var act = new TacticalAct(actScheme);
                    actList.Add(act);
                }
            }

            return actList.ToArray();
        }
    }
}
