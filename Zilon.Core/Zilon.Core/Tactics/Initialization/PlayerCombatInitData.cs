using Zilon.Logic.Persons;
using Zilon.Logic.Players;

namespace Zilon.Logic.Tactics.Initialization
{
    public class PlayerCombatInitData
    {
        public IPlayer Player { get; set; }
        public Squad[] Squads { get; set; }
    }
}
