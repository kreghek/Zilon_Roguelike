namespace Zilon.Core.MapGenerators.RoomStyle
{
    public sealed class RoomMatrix
    {
        private readonly Room[,] _rooms;

        public RoomMatrix(int size)
        {
            _rooms = new Room[size, size];
        }

        public void SetRoom(int x, int y, Room room)
        {
            _rooms[x, y] = room;
        }

        public Room GetRoom(int x, int y)
        {
            return _rooms[x, y];
        }
    }
}
