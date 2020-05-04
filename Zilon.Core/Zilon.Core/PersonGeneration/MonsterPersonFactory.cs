using System;
using System.Linq;

using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;

namespace Zilon.Core.PersonGeneration
{
    public sealed class MonsterPersonFactory : IMonsterPersonFactory
    {
        public IPerson Create(IMonsterScheme monsterScheme)
        {
            var monsterPerson = new MonsterPerson(monsterScheme);


            var Acts = new ITacticalAct[] {
                    new MonsterTacticalAct(monsterScheme.PrimaryAct)
                };

            var combaActModule = new MonsterCombatActModule(Acts);

            monsterPerson.AddModule(combaActModule);

            var defenses = monsterScheme.Defense?.Defenses?
                .Select(x => new PersonDefenceItem(x.Type, x.Level))
                .ToArray();

            var defenceStats = new PersonDefenceStats(
                    defenses ?? Array.Empty<PersonDefenceItem>(),
                    Array.Empty<PersonArmorItem>());

            var combatStatsModule = new MonsterCombatStatsModule(defenceStats);
            monsterPerson.AddModule(combatStatsModule);

            var survivalModule = new MonsterSurvivalModule(monsterScheme);
            monsterPerson.AddModule(survivalModule);

            var diseaseModule = new DiseaseModule();
            monsterPerson.AddModule(diseaseModule);

            return monsterPerson;
        }
    }
}
