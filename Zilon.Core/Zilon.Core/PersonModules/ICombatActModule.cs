using Zilon.Core.Persons;

namespace Zilon.Core.PersonModules
{
    public interface ICombatActModule: IPersonModule
    {
        /// <summary>
        /// Набор всех действий.
        /// </summary>
        ITacticalAct[] Acts { get; set; }
    }
}
