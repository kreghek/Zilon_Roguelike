﻿using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Common.Schemes
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class TestSectorRoomMapFactoryOptionsSubScheme : ISectorRoomMapFactoryOptionsSubScheme
    {
        public int RegionCount { get; set; }
        public int RegionSize { get; set; }
        public SchemeSectorMapGenerator MapGenerator => SchemeSectorMapGenerator.Room;
    }
}