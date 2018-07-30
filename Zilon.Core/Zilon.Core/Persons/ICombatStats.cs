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
    }
}
