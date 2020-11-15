using Zilon.Core.Persons;

namespace Zilon.Core.PersonModules
{
    public interface ICombatActModule : IPersonModule
    {
        /// <summary>
        /// Рассчёт всех действий, используемых в бою.
        /// </summary>
        IEnumerable<ITacticalAct> CalcCombatActs();
    }
}