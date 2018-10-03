using Zilon.Core.Components;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Характеристики, используемые персонажем в бою.
    /// </summary>
    public interface ICombatStats
    {
        /// <summary>
        /// Перечень навыков.
        /// </summary>
        CombatStatItem[] Stats { get; }

        /// <summary>
        /// Навыки оборонятся против наступательных действий.
        /// </summary>
        IPersonDefenceStats DefenceStats { get; }
    }
}
