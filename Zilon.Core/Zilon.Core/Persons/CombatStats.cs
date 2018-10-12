namespace Zilon.Core.Persons
{
    /// <summary>
    /// Базовая реализация набора навыков для боя.
    /// </summary>
    public class CombatStats : ICombatStats
    {
        public CombatStats()
        {
            DefenceStats = new PersonDefenceStats(new PersonDefenceItem[0], new PersonArmorItem[0]);
        }

        /// <summary>
        /// Навыки обороны против наступательных действий.
        /// </summary>
        public IPersonDefenceStats DefenceStats { get; set; }
    }
}
