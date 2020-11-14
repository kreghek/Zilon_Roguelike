using Zilon.Core.Persons;

namespace Zilon.Core.PersonModules
{
    /// <summary>
    ///     Характеристики, используемые персонажем в бою.
    /// </summary>
    public interface ICombatStatsModule : IPersonModule
    {
        /// <summary>
        ///     Навыки оборонятся против наступательных действий.
        /// </summary>
        IPersonDefenceStats DefenceStats { get; }
    }
}