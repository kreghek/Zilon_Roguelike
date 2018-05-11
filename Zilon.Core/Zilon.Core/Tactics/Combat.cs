namespace Zilon.Core.Tactics
{
    using System;

    using Zilon.Core.Players;
    using Zilon.Core.Tactics.Map;

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
            if (squad == null)
            {
                throw new ArgumentNullException(nameof(squad));
            }

            if (targetNode == null)
            {
                throw new ArgumentNullException(nameof(targetNode));
            }

            squad.SetCurrentNode(targetNode);
        }
    }
}
