using System.Collections.Generic;

using Zilon.Core.Persons;

namespace Zilon.Core.PersonModules
{
    public interface ICombatActModule : IPersonModule
    {
        bool IsCombatMode { get; set; }

        /// <summary>
        /// Gets all available combat act.
        /// </summary>
        IEnumerable<ICombatAct> GetCurrentCombatActs();
    }
}