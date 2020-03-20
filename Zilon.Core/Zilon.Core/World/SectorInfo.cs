using System;

using Zilon.Core.Tactics;

namespace Zilon.Core.World
{
    public class SectorInfo
    {
        public SectorInfo(ISector sector,
                          Province province,
                          ProvinceNode provinceNode)
        {
            Sector = sector ?? throw new ArgumentNullException(nameof(sector));
            Province = province ?? throw new ArgumentNullException(nameof(province));
            ProvinceNode = provinceNode ?? throw new ArgumentNullException(nameof(provinceNode));
        }

        public ISector Sector { get; }
        public Province Province { get; }
        public ProvinceNode ProvinceNode { get; }
    }
}
