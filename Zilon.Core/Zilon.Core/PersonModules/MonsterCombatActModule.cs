using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Persons;

namespace Zilon.Core.PersonModules
{
    public sealed class MonsterCombatActModule : ICombatActModule
    {
        private readonly IEnumerable<ITacticalAct> _acts;

        public MonsterCombatActModule(IEnumerable<ITacticalAct> acts)
        {
            _acts = acts.ToArray();
        }

        public string Key { get => nameof(ICombatActModule); }
        public bool IsActive { get; set; }

        public IEnumerable<ITacticalAct> CalcCombatActs()
        {
            return _acts;
        }
    }
}
