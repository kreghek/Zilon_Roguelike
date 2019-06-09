using System;

namespace Zilon.Core.WorldGeneration
{
    [Flags]
    public enum GlobeRegionDraftValueType
    {
        Wild = 1 << 0,
        Town = 1 << 1,
        Dungeon = 1 << 2
    }
}
