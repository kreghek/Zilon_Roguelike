using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Persons;

namespace Zilon.Core.PersonModules
{
    /// <summary>
    /// Реализация модуля боевых действий для монстра.
    /// Для монстров список действий фиксирован в зависимости от типа монстра.
    /// В отличии от базовой реализации действия не рассчитываются в зависимости от экипировки.
    /// </summary>
    public sealed class MonsterCombatActModule : ICombatActModule
    {
        private readonly IEnumerable<ITacticalAct> _acts;

        public MonsterCombatActModule(IEnumerable<ITacticalAct> acts)
        {
            _acts = acts.ToArray();
        }

        /// <inheritdoc/>
        public string Key => nameof(ICombatActModule);

        /// <inheritdoc/>
        public bool IsActive { get; set; }

        /// <inheritdoc/>
        public IEnumerable<ITacticalAct> CalcCombatActs()
        {
            return _acts;
        }
    }
}