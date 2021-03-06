﻿using JsonSubTypes;

using Newtonsoft.Json;

namespace Zilon.Core.Schemes
{
    [JsonConverter(typeof(JsonSubtypes), nameof(MapGenerator))]
    [JsonSubtypes.KnownSubType(
        typeof(SectorCellularAutomataMapFactoryOptionsSubScheme),
        nameof(SchemeSectorMapGenerator.CellularAutomaton)
    )]
    [JsonSubtypes.KnownSubType(
        typeof(SectorRoomMapFactoryOptionsSubScheme),
        nameof(SchemeSectorMapGenerator.Room)
    )]
    [JsonSubtypes.KnownSubType(
        typeof(SectorOpenMapFactoryOptionsSubScheme),
        nameof(SchemeSectorMapGenerator.Open)
    )]
    public abstract class SectorMapFactoryOptionsSubSchemeBase : ISectorMapFactoryOptionsSubScheme
    {
        public abstract SchemeSectorMapGenerator MapGenerator { get; }
    }
}