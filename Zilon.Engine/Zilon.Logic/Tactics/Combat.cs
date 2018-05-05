using System;
using Zilon.Logic.Players;
using Zilon.Logic.Tactics.Map;

namespace Zilon.Logic.Tactics
{
    public class Combat
    {
        public IPlayer[] Logins { get; set; }
        public bool Started { get; set; }
        public CombatMap Map { get; set; }
        public Actor[] Actors { get; set; }
        public int GameTurn { get; set; }
        public DateTime LastTime { get; set; }
        //public MapEvent MapEvent { get; set; }
        //public CombatTrigger[] Triggers { get; set; }
        //public Dictionary<string, AccountTask[]> AccountTaskDict { get; set; }
    }
}
