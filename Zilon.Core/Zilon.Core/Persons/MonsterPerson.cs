using System;
using System.Linq;

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

        public ICombatStats CombatStats { get; }

        public IPropStore Inventory => throw new NotSupportedException("Для монстров не поддерживается инвентарь.");

        /// <summary>
        /// Монстры не нуждаются в данных о выживании. Условно, у них всегда всего хватает.
        /// </summary>
        public ISurvivalData Survival => null;

        public EffectCollection Effects { get; }
        public MonsterScheme Scheme { get; }

        public MonsterPerson(MonsterScheme scheme)
        {
            Scheme = scheme;

            Hp = scheme.Hp;
            TacticalActCarrier = new TacticalActCarrier
            {
                Acts = new ITacticalAct[] {
                    new MonsterTacticalAct(scheme.PrimaryAct)
                }
            };

            var defences = scheme.Defence?.Defences?
                .Select(x => new PersonDefenceItem(x.Defence, x.Level))
                .ToArray();

            CombatStats = new CombatStats
            {
                DefenceStats = new PersonDefenceStats
                {
                    Defences = defences
                }
            };

            Effects = new EffectCollection();
        }
    }
}