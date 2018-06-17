namespace Zilon.Core.Tactics.Initialization
{
    using Zilon.Core.Tactics.Spatial;

    public class CombatInitData
    {
        public PlayerCombatInitData[] Players { get; set; }
        public HexMap Map { get; set; }
    }
}
