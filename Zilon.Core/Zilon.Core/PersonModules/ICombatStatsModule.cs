using System;

using Zilon.Core.Persons;

namespace Zilon.Core.PersonModules
{
    /// <summary>
    /// Характеристики, используемые персонажем в бою.
    /// </summary>
    public interface ICombatStatsModule: IPersonModule
    {
        /// <summary>
        /// Навыки оборонятся против наступательных действий.
        /// </summary>
        IPersonDefenceStats DefenceStats { get; }
    }

    /// <summary>
    /// Базовая реализация набора навыков для боя.
    /// </summary>
    public sealed class CombatStatsModule : ICombatStatsModule
    {
        public CombatStatsModule()
        {
            DefenceStats = new PersonDefenceStats(Array.Empty<PersonDefenceItem>(), Array.Empty<PersonArmorItem>());
            IsActive = true;
        }

        /// <summary>
        /// Навыки обороны против наступательных действий.
        /// </summary>
        public IPersonDefenceStats DefenceStats { get; set; }

        /// <inheritdoc/>
        public string Key { get => nameof(ICombatStatsModule); }

        /// <inheritdoc/>
        public bool IsActive { get; set; }
    }
}
