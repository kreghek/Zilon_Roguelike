namespace Zilon.Core.Tactics.Map
{
    using System.Collections.Generic;

    public class NodeHelper
    {
        private const float LOCATION_DISTANCE = 20;

        public static MapNode[] GetSquadNodes(MapNode teamNode, IEnumerable<MapNode> nodes)
        {
            var result = new List<MapNode>();

            foreach (var node in nodes)
            {
                var xComponent = node.Position.X - teamNode.Position.X;
                var yComponent = node.Position.Y - teamNode.Position.Y;
                var distance = System.Math.Sqrt(System.Math.Pow(xComponent, 2) + System.Math.Pow(yComponent, 2));

                if (distance <= LOCATION_DISTANCE * 1.6f)
                {
                    result.Add(node);
                }
            }

            return result.ToArray();
        }
    }
}