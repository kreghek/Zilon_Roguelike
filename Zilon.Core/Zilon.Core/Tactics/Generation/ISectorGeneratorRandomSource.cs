using System.Collections.Generic;

namespace Zilon.Core.Tactics.Generation
{
    public interface ISectorGeneratorRandomSource
    {
        OffsetCoords RollRoomPosition(int maxPosition);
        Size RollRoomSize(int maxSize);
        Room[] RollConnectedRooms(Room room, IList<Room> rooms);
    }
}
