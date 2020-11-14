using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;

namespace Zilon.Core.PersonGeneration
{
    public sealed class MonsterPersonFactory : IMonsterPersonFactory
    {
        public IPerson Create(IMonsterScheme monsterScheme)
        {
            MonsterPerson monsterPerson = new MonsterPerson(monsterScheme);

            ITacticalAct[] Acts = new ITacticalAct[] {new MonsterTacticalAct(monsterScheme.PrimaryAct)};

            MonsterCombatActModule combaActModule = new MonsterCombatActModule(Acts);

            monsterPerson.AddModule(combaActModule);

            var defenses = monsterScheme.Defense?.Defenses?
                .Select(x => new PersonDefenceItem(x.Type, x.Level))
                .ToArray();

            PersonDefenceStats defenceStats = new PersonDefenceStats(
                defenses ?? Array.Empty<PersonDefenceItem>(),
                Array.Empty<PersonArmorItem>());

            MonsterCombatStatsModule combatStatsModule = new MonsterCombatStatsModule(defenceStats);
            monsterPerson.AddModule(combatStatsModule);

            MonsterSurvivalModule survivalModule = new MonsterSurvivalModule(monsterScheme);
            monsterPerson.AddModule(survivalModule);

            MonsterDiseaseModule diseaseModule = new MonsterDiseaseModule();
            monsterPerson.AddModule(diseaseModule);

            return monsterPerson;
        }
    }
}