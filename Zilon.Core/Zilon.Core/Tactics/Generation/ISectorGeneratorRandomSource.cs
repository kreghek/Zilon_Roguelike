using System.Collections.Generic;

namespace Zilon.Core.Tactics.Generation
{
    public interface ISectorGeneratorRandomSource
    {
        void RollRoomPosition(int maxPosition, out int x, out int y);
        void RollRoomSize(int maxSize, out int w, out int h);
        Room RollConnectedRoom(Room room, List<Room> rooms);
    }
}
