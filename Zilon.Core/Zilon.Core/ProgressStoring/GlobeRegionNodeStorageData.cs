using Zilon.Core.World;

namespace Zilon.Core.ProgressStoring
{
    public sealed class GlobeRegionNodeStorageData
    {
        public OffsetCoords Coords { get; set; }

        public string SchemeSid { get; set; }

        public bool IsHome { get; set; }

        public bool IsTown { get; set; }

        public bool IsBorder { get; set; }

        public bool IsStart { get; set; }

        public GlobeRegionNodeMonsterStateStorageData MonsterState { get; set; }

        public GlobeNodeObservedState Observed { get; set; }

        public static GlobeRegionNodeStorageData Create(ProvinceNode globeRegionNode)
        {
            var storageData = new GlobeRegionNodeStorageData();

            storageData.Coords = new OffsetCoords(globeRegionNode.OffsetX, globeRegionNode.OffsetY);
            storageData.SchemeSid = globeRegionNode.Scheme.Sid;
            storageData.IsHome = globeRegionNode.IsHome;
            storageData.IsTown = globeRegionNode.IsTown;
            storageData.IsBorder = globeRegionNode.IsBorder;
            storageData.IsStart = globeRegionNode.IsStart;

            storageData.MonsterState = new GlobeRegionNodeMonsterStateStorageData { };
            storageData.Observed = globeRegionNode.ObservedState;

            return storageData;
        }
    }
}
