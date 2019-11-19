using System;
using System.Collections.Generic;
using System.Linq;
using Zilon.Core.Persons;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.ProgressStoring
{
    public sealed class SectorStorageData
    {
        public string SchemeSid { get; private set; }
        public OffsetCoords[] PassMap { get; private set; }

        public OffsetCoords[] Obstacles { get; private set; }

        public OffsetCoords[][] Regions { get; private set; }

        public TransitionStorageData[] Transitions { get; private set; }


        public static SectorStorageData Create(ISector sector, IDictionary<IPerson, string> humanPersonDict)
        {
            if (sector is null)
            {
                throw new ArgumentNullException(nameof(sector));
            }

            var storageData = new SectorStorageData();

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
                Sid = x.Value.SectorSid
            }).ToArray();

            return storageData;
        }

        public sealed class TransitionStorageData
        {
            public OffsetCoords Coords { get; set; }

            public string Sid { get; set; }
        }
    }
}
