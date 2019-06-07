using System;

namespace Zilon.Core.WorldGeneration
{
    [Flags]
    public enum GlobeRegionDraftValueType
    {
        Wild = 0,
        Town = 1 << 0,
        Dungeon = 1 << 1
    }
}
