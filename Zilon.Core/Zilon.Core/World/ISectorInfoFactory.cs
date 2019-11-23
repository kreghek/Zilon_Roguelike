using Zilon.Core.ProgressStoring;

namespace Zilon.Core.World
{
    public interface ISectorInfoFactory
    {
        SectorInfo Create(GlobeRegion globeRegion, GlobeRegionNode globeRegionNode, SectorStorageData sectorStorageData);
    }
}