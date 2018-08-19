using System;

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

        public ICombatStats CombatStats => throw new NotSupportedException("Для монстров не поддерживаются отдельные характеристики");

        public IPropStore Inventory => throw new NotSupportedException("Для монстров не поддерживается инвентарь.");

        /// <summary>
        /// Монстры не нуждаются в данных о выживании. Условно, у них всегда всего хватает.
        /// </summary>
        public ISurvivalData Survival => null;

        public EffectCollection Effects { get; }

        public MonsterPerson(MonsterScheme scheme)
        {
            Hp = 27;
            TacticalActCarrier = new TacticalActCarrier
            {
                Acts = new ITacticalAct[] {
                    new MonsterTacticalAct(scheme.PrimaryAct, 1)
                }
            };

            Effects = new EffectCollection();
        }
    }
}