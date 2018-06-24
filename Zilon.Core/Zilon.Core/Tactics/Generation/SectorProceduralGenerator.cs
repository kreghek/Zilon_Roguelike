using System;
using System.Collections.Generic;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Generation
{
    public class SectorProceduralGenerator
    {
        private const int ROOM_COUNT = 10;

        private readonly ISectorGeneratorRandomSource _randomSource;

        public SectorProceduralGenerator(ISectorGeneratorRandomSource randomSource)
        {
            _randomSource = randomSource;
        }

        public void Generate(ISector sector, IMap map, IActor playerActor)
        {
            var roomGridSize = (int)Math.Ceiling(Math.Log(ROOM_COUNT, 2)) + 1;
            var roomGrid = new Room[roomGridSize, roomGridSize];
            var roomStack = new Stack<Room>();

            var currentRoom = new Room()
            {
            };

            roomStack.Push(currentRoom);

            for (var i = 0; i < ROOM_COUNT; i++)
            {
                var attemptCounter = 3;
                while (true)
                {
                    _randomSource.RollRoomPosition(out int roomPositionX, out int roomPositionY);

                    var currentRoom = roomGrid[roomPositionX, roomPositionY];
                    if (currentRoom == null)
                    {
                        var room = new Room();
                        room.PositionX = roomPositionX;
                        room.PositionY = roomPositionY;

                        _randomSource.RollRoomSize(out int roomWidth, out int roomHeight);

                        room.Width = roomWidth;
                        room.Height = roomHeight;

                        roomStack.Push(room);
                    }
                    else
                    {
                        break;
                    }

                    attemptCounter--;
                    if (attemptCounter <= 0)
                    {
                        throw new Exception("Не удалось выбрать ячейку для комнаты.");
                    }
                }
            }
        }
    }
}
