using System.Collections.Generic;

using Zilon.Core.Persons;

namespace Zilon.Core.PersonModules
{
    public interface ICombatActModule : IPersonModule
    {
        bool IsCombatMode { get; set; }

        /// <summary>
        /// Рассчёт всех действий, используемых в бою.
        /// </summary>
        IEnumerable<ITacticalAct> CalcCombatActs();
    }
}