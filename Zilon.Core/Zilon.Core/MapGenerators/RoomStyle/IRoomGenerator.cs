using System.Collections.Generic;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators.RoomStyle
{
    public interface IRoomGenerator
    {
        void BuildRoomCorridors(IMap map, List<Room> rooms, HashSet<string> edgeHash);
        void CreateRoomNodes(IMap map, List<Room> rooms, HashSet<string> edgeHash);
        List<Room> GenerateRoomsInGrid();
    }
}