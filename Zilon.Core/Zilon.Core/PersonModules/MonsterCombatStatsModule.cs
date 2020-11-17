using Zilon.Core.Persons;

namespace Zilon.Core.PersonModules
{
    /// <summary>
    /// Реализация модуля боевых характеристик для монстров.
    /// В отличии от базовой реализации, характристики монстров фиксированы, а не рассчитываются исходя из экипировки.
    /// </summary>
    public sealed class MonsterCombatStatsModule : ICombatStatsModule
    {
        public MonsterCombatStatsModule(IPersonDefenceStats defenceStats)
        {
            DefenceStats = defenceStats;
        }

        /// <inheritdoc/>
        public IPersonDefenceStats DefenceStats { get; }

        /// <inheritdoc/>
        public string Key { get => nameof(ICombatStatsModule); }

        /// <inheritdoc/>
        public bool IsActive { get; set; }
    }
}