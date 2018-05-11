namespace Zilon.Core.Tactics.Map
{
    using System;
    using System.Collections.Generic;

    using Zilon.Core.Math;

    public class CombatMap : ICombatMap
    {
        public List<MapNode> Nodes { get; set; }
        public List<MapNode> TeamNodes { get; set; }

        public bool IsPositionAvailableFor(MapNode targetNode, ActorSquad actorSquad)
        {
            return true;
        }

        public void ReleaseNode(MapNode node, ActorSquad actorSquad)
        {
            
        }

        public void HoldNode(MapNode node, ActorSquad actorSquad)
        {
            
        }
    }
}
