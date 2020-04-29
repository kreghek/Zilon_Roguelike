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
    public class MonsterPerson : PersonBase
    {
        public override int Id { get; set; }
        public int Hp { get; }

        public override ITacticalActCarrier TacticalActCarrier { get; }

        public override IEvolutionData EvolutionData => throw new NotSupportedException("Для монстров не поддерживается развитие");

        public override ICombatStats CombatStats { get; }

        public override ISurvivalData Survival { get; }

        public override EffectCollection Effects { get; }

        public IMonsterScheme Scheme { get; }

        public override PhysicalSize PhysicalSize { get => PhysicalSize.Size1; }
        public override bool HasInventory { get => false; }
        public override IDiseaseData DiseaseData { get; }

        public MonsterPerson([NotNull] IMonsterScheme scheme) : base()
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
                    defenses ?? Array.Empty<PersonDefenceItem>(),
                    Array.Empty<PersonArmorItem>())
            };

            Survival = new MonsterSurvivalData(scheme);

            Effects = new EffectCollection();

            DiseaseData = new DiseaseData();
        }

        public override string ToString()
        {
            return $"{Scheme?.Name?.En}";
        }
    }
}