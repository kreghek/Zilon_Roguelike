using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Common;
using Zilon.Core.World;
using Zilon.Core.WorldGeneration;

namespace Zilon.Core.Commands.Globe
{
    public static class RegionTransitionHelper
    {
        public static IEnumerable<GlobeRegionNode> GetNeighborBorderNodes(GlobeRegionNode currentTerrainNode,
            TerrainCell currentTerrainCell,
            IEnumerable<GlobeRegionNode> targetRegionBorderNodes,
            TerrainCell targetNeighborTerrainCell)
        {
            var regionNodeOffsetX = targetNeighborTerrainCell.Coords.X - currentTerrainCell.Coords.X;
            var regionNodeOffsetY = targetNeighborTerrainCell.Coords.Y - currentTerrainCell.Coords.Y;

            var targetRegionBorderNodeOffsetCoords = targetRegionBorderNodes.Select(node =>
                new
                {
                    RegionOffsetCoords = new OffsetCoords(
                    node.OffsetX + regionNodeOffsetX * 20,
                    node.OffsetY + regionNodeOffsetY * 20
                    ),
                    Node = node
                });

            var transitionNodeCoords = targetRegionBorderNodeOffsetCoords.Where(coords =>
                HexHelper.ConvertToCube(coords.RegionOffsetCoords.X, coords.RegionOffsetCoords.Y)
                .DistanceTo(currentTerrainNode.CubeCoords) <= 1);

            return transitionNodeCoords.Select(x => x.Node);
        }
    }
}
