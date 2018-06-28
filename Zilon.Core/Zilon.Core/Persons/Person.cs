namespace Zilon.Core.Persons
{
    public class Person : IPerson, IEquipCarrier, ITacticalActCarrier
    {
        public int Id { get; set; }

        public float Hp { get; set; }

        public Equipment[] Equipments { get; }

        public ITacticalAct DefaultAct { get; }

        public ITacticalAct[] Acts { get; }
    }
}
