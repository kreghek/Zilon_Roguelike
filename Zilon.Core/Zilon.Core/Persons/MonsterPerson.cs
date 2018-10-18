using System;
using System.Linq;

using JetBrains.Annotations;

using Zilon.Core.Props;
using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Персонаж для монстров  секторе.
    /// </summary>
    public class MonsterPerson : IPerson
    {
        public int Id { get; set; }
        public int Hp { get; }
        public IEquipmentCarrier EquipmentCarrier => throw new NotSupportedException("Для монстров не поддерживается явная экипировка");

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

            var defences = scheme.Defence?.Defences?
                .Select(x => new PersonDefenceItem(x.Type, x.Level))
                .ToArray();

            CombatStats = new CombatStats
            {
                DefenceStats = new PersonDefenceStats(
                    defences ?? new PersonDefenceItem[0],
                    new PersonArmorItem[0])
            };

            Survival = SurvivalData.CreateMonsterPersonSurvival(scheme);

            Effects = new EffectCollection();
        }
    }
}