using Zilon.Logic.Tactics.Map;

namespace Zilon.Logic.Tactics.Initialization
{
    public class CombatInitData
    {
        public PlayerCombatInitData[] Players { get; set; }
        public CombatMap Map { get; set; }
    }
}
