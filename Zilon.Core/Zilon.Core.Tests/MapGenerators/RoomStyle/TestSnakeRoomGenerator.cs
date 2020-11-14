using System.Collections.Generic;
using System.Linq;

using Zilon.Core.MapGenerators;
using Zilon.Core.MapGenerators.RoomStyle;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tests.MapGenerators.RoomStyle
{
    internal sealed class TestSnakeRoomGenerator : RoomGeneratorBase
    {
        private readonly OffsetCoords[] _rolledOffsetCoords;

        public TestSnakeRoomGenerator()
        {
            _rolledOffsetCoords = new[]
            {
                new OffsetCoords(0, 0), new OffsetCoords(1, 0), new OffsetCoords(2, 0), new OffsetCoords(3, 0),
                new OffsetCoords(3, 1), new OffsetCoords(2, 1), new OffsetCoords(1, 1), new OffsetCoords(0, 1),
                new OffsetCoords(0, 2), new OffsetCoords(1, 2)
            };
        }

        public override IEnumerable<Room> GenerateRoomsInGrid(
            int roomCount,
            int roomMinSize,
            int roomMaxSize,
            IEnumerable<RoomTransition> availableTransitions)
        {
            var rooms = new List<Room>();

            for (var i = 0; i < _rolledOffsetCoords.Length; i++)
            {
                var room = new Room
                {
                    PositionX = _rolledOffsetCoords[i].X,
                    PositionY = _rolledOffsetCoords[i].Y
                };

                var rolledSize = new Size(3, 3);

                room.Width = rolledSize.Width + 2;
                room.Height = rolledSize.Height + 2;

                rooms.Add(room);
            }

            return rooms;
        }

        public override void BuildRoomCorridors(IMap map, IEnumerable<Room> rooms, HashSet<string> edgeHash)
        {
            var roomArray = rooms.ToArray();
            for (var i = 0; i < roomArray.Length - 1; i++)
            {
                ConnectRoomsWithCorridor(map, roomArray[i], roomArray[i + 1], edgeHash);
            }
        }

        public override void CreateRoomNodes(ISectorMap map, IEnumerable<Room> rooms, HashSet<string> edgeHash)
        {
            foreach (var room in rooms)
            {
                CreateOneRoomNodes(map, edgeHash, room);
            }
        }

        private void CreateOneRoomNodes(IMap map, HashSet<string> edgeHash, Room room)
        {
            for (var x = 0; x < room.Width; x++)
            {
                for (var y = 0; y < room.Height; y++)
                {
                    var nodeX = x + (room.PositionX * 20);
                    var nodeY = y + (room.PositionY * 20);
                    var node = new HexNode(nodeX, nodeY);
                    room.Nodes.Add(node);
                    map.AddNode(node);

                    RoomHelper.AddAllNeighborToMap(map, edgeHash, room, node);
                }
            }
        }
    }
}