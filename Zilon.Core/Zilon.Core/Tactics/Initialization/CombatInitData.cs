namespace Zilon.Core.Tactics.Initialization
{
    using Zilon.Core.Tactics.Map;

    public class CombatInitData
    {
        public PlayerCombatInitData[] Players { get; set; }
        public CombatMap Map { get; set; }
    }
}
