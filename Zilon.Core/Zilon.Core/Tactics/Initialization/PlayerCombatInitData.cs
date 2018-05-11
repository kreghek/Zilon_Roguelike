namespace Zilon.Core.Tactics.Initialization
{
    using Zilon.Core.Persons;
    using Zilon.Core.Players;

    public class PlayerCombatInitData
    {
        public IPlayer Player { get; set; }
        public Squad[] Squads { get; set; }
    }
}
