using System;

namespace Zilon.Core.Persons
{
    public class MonsterPerson: IPerson
    {
        public int Id { get; set; }
        public float Hp { get; }
        public IEquipmentCarrier EquipmentCarrier => throw new NotSupportedException("Для монстров не поддерживается явная экипировка");
    }
}
