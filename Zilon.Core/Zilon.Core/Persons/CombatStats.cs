using Zilon.Core.Components;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Базовая реализация набора навыков для боя.
    /// </summary>
    public class CombatStats : ICombatStats
    {
        /// <summary>
        /// Перечень навыков.
        /// </summary>
        public CombatStatItem[] Stats { get; set; }

        /// <summary>
        /// Навыки обороны против наступательных действий.
        /// </summary>
        public IPersonDefenceStats DefenceStats { get; }
    }
}
