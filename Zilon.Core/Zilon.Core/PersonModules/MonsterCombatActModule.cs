using System;
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
        private readonly IEnumerable<ICombatAct> _acts;

        public MonsterCombatActModule(IEnumerable<ICombatAct> acts)
        {
            _acts = acts.ToArray();
        }

        /// <inheritdoc />
        public string Key => nameof(ICombatActModule);

        /// <inheritdoc />
        public bool IsActive { get; set; }

        public bool IsCombatMode { get; set; }

        public event EventHandler? CombatBegan;

        public void BeginCombat()
        {
        }

        public void EndCombat()
        {
        }

        /// <inheritdoc />
        public IEnumerable<ICombatAct> GetCurrentCombatActs()
        {
            return _acts;
        }

        public void Update()
        {
        }

        public void UseAct(ICombatAct combatAct)
        {
        }
    }
}