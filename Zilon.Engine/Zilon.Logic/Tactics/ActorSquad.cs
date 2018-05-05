using System.Collections.Generic;
using Zilon.Logic.Tactics.Map;

namespace Zilon.Logic.Tactics
{
    public class ActorSquad
    {
        public List<Actor> Actors { get; set; }
        public MapNode CurrentNode { get; set; }

        public ActorSquad() {
            Actors = new List<Actor>();
        }
    }
}
