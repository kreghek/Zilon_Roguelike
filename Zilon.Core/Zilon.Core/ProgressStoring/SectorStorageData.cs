using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Persons;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.World;

namespace Zilon.Core.ProgressStoring
{
    public sealed class SectorStorageData
    {
        public string Id { get; set; }

        public string SchemeSid { get; set; }
        public OffsetCoords[] PassMap { get; set; }

        public OffsetCoords[] Obstacles { get; set; }

        public OffsetCoords[][] Regions { get; set; }

        public TransitionStorageData[] Transitions { get; set; }

        public OffsetCoords TerrainCoords { get; set; }

        public OffsetCoords GlobeRegionNodeCoords { get; set; }


        public static SectorStorageData Create(Province globeRegion,
            ProvinceNode globeRegionNode,
            ISector sector,
            IDictionary<IPerson, string> humanPersonDict)
        {
            throw new NotImplementedException("Работа с миром изменилась. Нужно адаптировать.");

            if (globeRegion is null)
            {
                throw new ArgumentNullException(nameof(globeRegion));
            }

            if (globeRegionNode is null)
            {
                throw new ArgumentNullException(nameof(globeRegionNode));
            }

            if (sector is null)
            {
                throw new ArgumentNullException(nameof(sector));
            }

            var storageData = new SectorStorageData();

            storageData.Id = Guid.NewGuid().ToString();

            storageData.SchemeSid = sector.Scheme?.Sid;

            var passMapList = new List<OffsetCoords>();
            var obstacleList = new List<OffsetCoords>();
            foreach (HexNode node in sector.Map.Nodes)
            {
                var coords = new OffsetCoords(node.OffsetX, node.OffsetY);
                passMapList.Add(coords);

                if (node.IsObstacle)
                {
                    obstacleList.Add(coords);
                }
            }

            var regions = new List<OffsetCoords[]>();
            foreach (var region in sector.Map.Regions)
            {
                var regionNodes = region.Nodes.Cast<HexNode>().Select(x => new OffsetCoords(x.OffsetX, x.OffsetY)).ToArray();
                regions.Add(regionNodes);
            }

            storageData.Regions = regions.ToArray();

            storageData.PassMap = passMapList.ToArray();
            storageData.Obstacles = obstacleList.ToArray();
            storageData.Transitions = sector.Map.Transitions.Select(x => new TransitionStorageData
            {
                Coords = new OffsetCoords((x.Key as HexNode).OffsetX, (x.Key as HexNode).OffsetY),
                Sid = x.Value.SectorLevelSid
            }).ToArray();

            //storageData.TerrainCoords = globeRegion.GlobeCoords.Coords;
            storageData.GlobeRegionNodeCoords = new OffsetCoords(globeRegionNode.OffsetX, globeRegionNode.OffsetY);

            return storageData;
        }

        public sealed class TransitionStorageData
        {
            public OffsetCoords Coords { get; set; }

            public string Sid { get; set; }
        }
    }
}
