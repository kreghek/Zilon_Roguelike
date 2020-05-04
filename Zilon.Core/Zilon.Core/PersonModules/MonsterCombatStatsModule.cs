using Zilon.Core.Persons;

namespace Zilon.Core.PersonModules
{
    public sealed class MonsterCombatStatsModule : ICombatStatsModule
    {
        public MonsterCombatStatsModule(IPersonDefenceStats defenceStats)
        {
            DefenceStats = defenceStats;
        }

        public IPersonDefenceStats DefenceStats { get; }
        public string Key { get => nameof(ICombatStatsModule); }
        public bool IsActive { get; set; }
    }
}
