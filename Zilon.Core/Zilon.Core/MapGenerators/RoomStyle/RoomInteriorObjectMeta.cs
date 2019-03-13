using System;

namespace Zilon.Core.MapGenerators.RoomStyle
{
    public sealed class RoomInteriorObjectMeta
    {
        public RoomInteriorObjectMeta(OffsetCoords coords)
        {
            Coords = coords ?? throw new ArgumentNullException(nameof(coords));
        }

        public OffsetCoords Coords { get; }
    }
}
