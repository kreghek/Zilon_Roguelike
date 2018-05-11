namespace Zilon.Core.Tactics.Map
{
    using System.Collections.Generic;

    public interface ICombatMap
    {
        List<MapNode> Nodes { get; set; }
        List<MapNode> TeamNodes { get; set; }
        bool IsPositionAvailableFor(MapNode targetNode, ActorSquad actorSquad);
        void ReleaseNode(MapNode node, ActorSquad actorSquad);
        void HoldNode(MapNode node, ActorSquad actorSquad);
    }
}