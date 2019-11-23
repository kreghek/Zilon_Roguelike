using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Common;
using Zilon.Core.World;
using Zilon.Core.World;

namespace Zilon.Core.Commands.Globe
{
    public static class RegionTransitionHelper
    {
        public static IEnumerable<GlobeRegionNode> GetNeighborBorderNodes(GlobeRegionNode currentTerrainNode,
            TerrainCell currentTerrainCell,
            IEnumerable<GlobeRegionNode> targetRegionBorderNodes,
            TerrainCell targetNeighborTerrainCell)
        {
            if (targetRegionBorderNodes == null)
            {
                throw new ArgumentNullException(nameof(targetRegionBorderNodes));
            }

            if (currentTerrainCell == null)
            {
                throw new ArgumentNullException(nameof(currentTerrainCell));
            }

            if (targetNeighborTerrainCell == null)
            {
                throw new ArgumentNullException(nameof(targetNeighborTerrainCell));
            }

            const int REGION_SIZE = 20;

            var regionNodeOffsetX = targetNeighborTerrainCell.Coords.X - currentTerrainCell.Coords.X;
            var regionNodeOffsetY = targetNeighborTerrainCell.Coords.Y - currentTerrainCell.Coords.Y;

            var targetRegionBorderNodeOffsetCoords = targetRegionBorderNodes.Select(node =>
                new
                {
                    RegionOffsetCoords = new OffsetCoords(
                        regionNodeOffsetX >= 0 ? node.OffsetX + regionNodeOffsetX * REGION_SIZE : -(REGION_SIZE - node.OffsetX),
                        regionNodeOffsetY >= 0 ? node.OffsetY + regionNodeOffsetY * REGION_SIZE : -(REGION_SIZE - node.OffsetY)
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
