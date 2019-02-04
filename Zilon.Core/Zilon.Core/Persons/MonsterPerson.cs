using System;
using System.Linq;

using JetBrains.Annotations;

using Zilon.Core.Props;
using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Персонаж для монстров в секторе.
    /// </summary>
    public class MonsterPerson : IPerson
    {
        public int Id { get; set; }
        public int Hp { get; }
        public IEquipmentCarrier EquipmentCarrier => null;

        public ITacticalActCarrier TacticalActCarrier { get; }

        public IEvolutionData EvolutionData => throw new NotSupportedException("Для монстров не поддерживается развитие");

        public ICombatStats CombatStats { get; }

        public IPropStore Inventory => throw new NotSupportedException("Для монстров не поддерживается инвентарь.");

        public ISurvivalData Survival { get; }

        public EffectCollection Effects { get; }

        public IMonsterScheme Scheme { get; }

        public MonsterPerson([NotNull] IMonsterScheme scheme)
        {
            
            Scheme = scheme ?? throw new ArgumentNullException(nameof(scheme));

            Hp = scheme.Hp;
            TacticalActCarrier = new TacticalActCarrier
            {
                Acts = new ITacticalAct[] {
                    new MonsterTacticalAct(scheme.PrimaryAct)
                }
            };

            var defenses = scheme.Defense?.Defenses?
                .Select(x => new PersonDefenceItem(x.Type, x.Level))
                .ToArray();

            CombatStats = new CombatStats
            {
                DefenceStats = new PersonDefenceStats(
                    defenses ?? new PersonDefenceItem[0],
                    new PersonArmorItem[0])
            };

            Survival = new MonsterSurvivalData(scheme);

            Effects = new EffectCollection();
        }

        public override string ToString()
        {
            return $"{Scheme} Id:{Id}";
        }
    }
}