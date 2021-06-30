using System;
using System.Collections.Generic;

using Zilon.Core.Persons;

namespace Zilon.Core.PersonModules
{
    public interface ICombatActModule : IPersonModule
    {
        bool IsCombatMode { get; set; }

        void BeginCombat();
        void EndCombat();

        /// <summary>
        /// Gets all available combat act.
        /// </summary>
        IEnumerable<ICombatAct> GetCurrentCombatActs();

        /// <summary>
        /// Update combat acts:
        /// - Update inner counters like changing act list counter.
        /// - Update state of all person combat acts.
        /// - Regenerate list of combat acts according the person adapt ability.
        /// </summary>
        void Update();

        void UseAct(ICombatAct combatAct);

        event EventHandler? CombatBegan;
    }
}