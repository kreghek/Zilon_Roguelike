using Zilon.Core.ProgressStoring;
using Zilon.Core.WorldGeneration;

namespace Zilon.Core.World
{
    public interface ISectorInfoFactory
    {
        SectorInfo Create(GlobeRegion globeRegion, GlobeRegionNode globeRegionNode, SectorStorageData sectorStorageData);
    }
}