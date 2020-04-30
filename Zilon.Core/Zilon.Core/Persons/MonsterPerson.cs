using System;
using System.Linq;

using JetBrains.Annotations;
using Zilon.Core.PersonModules;
using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Персонаж для монстров в секторе.
    /// </summary>
    public class MonsterPerson : PersonBase
    {
        /// <inheritdoc/>
        public override int Id { get; set; }

        /// <inheritdoc/>
        public int Hp { get; }

        /// <inheritdoc/>
        public override EffectCollection Effects { get; }

        /// <inheritdoc/>
        public IMonsterScheme Scheme { get; }

        /// <inheritdoc/>
        public override PhysicalSize PhysicalSize { get => PhysicalSize.Size1; }

        /// <inheritdoc/>
        public override IDiseaseData DiseaseData { get; }

        public MonsterPerson([NotNull] IMonsterScheme scheme) : base()
        {

            Scheme = scheme ?? throw new ArgumentNullException(nameof(scheme));

            Hp = scheme.Hp;
            var combaActModule = new CombatActModule
            {
                Acts = new ITacticalAct[] {
                    new MonsterTacticalAct(scheme.PrimaryAct)
                }
            };

            AddModule(combaActModule);

            var defenses = scheme.Defense?.Defenses?
                .Select(x => new PersonDefenceItem(x.Type, x.Level))
                .ToArray();

            var combatStatsModule = new CombatStatsModule
            {
                DefenceStats = new PersonDefenceStats(
                    defenses ?? Array.Empty<PersonDefenceItem>(),
                    Array.Empty<PersonArmorItem>())
            };
            AddModule(combatStatsModule);

            var survivalModule = new MonsterSurvivalModule(scheme);
            AddModule(survivalModule);

            Effects = new EffectCollection();

            DiseaseData = new DiseaseData();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{Scheme?.Name?.En}";
        }
    }
}