using System.Collections.Generic;

using Zilon.Core.Persons;
using Zilon.Core.ProgressStoring;

namespace Zilon.Core.World
{
    public interface ISectorInfoFactory
    {
        SectorInfo Create(Province globeRegion,
            ProvinceNode globeRegionNode,
            SectorStorageData sectorStorageData,
            IEnumerable<ActorStorageData> actors,
            IDictionary<string, IPerson> personDict);
    }
}