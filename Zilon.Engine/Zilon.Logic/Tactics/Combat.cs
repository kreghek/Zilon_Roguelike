using System;
using Zilon.Logic.Persons;
using Zilon.Logic.Players;
using Zilon.Logic.Tactics.Map;

namespace Zilon.Logic.Tactics
{
    public class Combat
    {
        public IPlayer[] Logins { get; set; }
        public bool Started { get; set; }
        public CombatMap Map { get; set; }
        public ActorSquad[] Squads { get; set; }
        public int GameTurn { get; set; }
        public DateTime LastTime { get; set; }
        //public MapEvent MapEvent { get; set; }
        //public CombatTrigger[] Triggers { get; set; }
        //public Dictionary<string, AccountTask[]> AccountTaskDict { get; set; }

        public void Move(ActorSquad squad, MapNode targetNode)
        {
            squad.SetCurrentNode(targetNode);
        }
    }
}
