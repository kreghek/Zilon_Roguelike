using System;
using System.Collections.Generic;
using Zilon.Logic.Tactics.Map;

namespace Zilon.Logic.Tactics
{
    public class ActorSquad
    {
        public List<Actor> Actors { get; set; }
        public MapNode Node { get; private set; }

        public event EventHandler NodeChanged;

        public ActorSquad(MapNode node)
        {
            Actors = new List<Actor>();
            Node = node;
        }

        public void SetCurrentNode(MapNode node)
        {
            Node = node;
            NodeChanged(this, new EventArgs());
        }
    }
}
