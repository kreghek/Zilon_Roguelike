using System;
using System.Collections.Generic;
using Zilon.Logic.Math;
using Zilon.Logic.Persons;
using Zilon.Logic.Tactics.Map;

namespace Zilon.Logic.Tactics
{
    public class ActorSquad
    {
        public int Id { get; private set; }

        public List<Actor> Actors { get; set; }
        public MapNode Node { get; private set; }
        public Squad Squad { get; private set; }

        public int MP { get; set; }

        public SquadTurnState TurnState { get; set; }
        public Vector2? LookAt { get; set; }

        public event EventHandler NodeChanged;

        public ActorSquad(int id, Squad squad, MapNode node)
        {
            Actors = new List<Actor>();
            Node = node;
            Squad = squad;
            MP = 1;
            Id = id;
        }

        public void SetCurrentNode(MapNode node)
        {
            Node = node;

            NodeChanged?.Invoke(this, new EventArgs());
        }
    }
}
