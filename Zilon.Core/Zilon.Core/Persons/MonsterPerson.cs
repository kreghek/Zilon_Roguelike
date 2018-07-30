using System;

using Zilon.Core.Common;
using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Персонаж для монстров  секторе.
    /// </summary>
    public class MonsterPerson : IPerson
    {
        public int Id { get; set; }
        public float Hp { get; }
        public IEquipmentCarrier EquipmentCarrier => throw new NotSupportedException("Для монстров не поддерживается явная экипировка");

        public ITacticalActCarrier TacticalActCarrier { get; }

        public IEvolutionData EvolutionData => throw new NotSupportedException("Для монстров не поддерживается развитие");

        public MonsterPerson()
        {
            TacticalActCarrier = new TacticalActCarrier();

            var scheme = new TacticalActScheme
            {
                Efficient = new Range<float>(1, 1),
                MinRange = 1,
                MaxRange = 1
            };

            TacticalActCarrier.Acts = new TacticalAct[] {
                new TacticalAct(1, scheme)
            };
        }
    }
}