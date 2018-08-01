namespace Zilon.Core.MapGenerators
{
    public static class GenerationHelper
    {
        public static OffsetCoords GetFreeCell(RoomMatrix grid, OffsetCoords rolled)
        {
            for (var i = 0; i < grid.Size; i++)
            {
                for (var j = 0; j < grid.Size; j++)
                {
                    if (IsValidForRoom(grid, rolled, i, j, 1, 1))
                    {
                        return new OffsetCoords(rolled.X + i * 1, rolled.Y + j * 1);
                    }

                    if (IsValidForRoom(grid, rolled, i, j, -1, 1))
                    {
                        return new OffsetCoords(rolled.X + i * -1, rolled.Y + j * 1);
                    }

                    if (IsValidForRoom(grid, rolled, i, j, 1, -1))
                    {
                        return new OffsetCoords(rolled.X + i * 1, rolled.Y + j * -1);
                    }

                    if (IsValidForRoom(grid, rolled, i, j, -1, -1))
                    {
                        return new OffsetCoords(rolled.X + i * -1, rolled.Y + j * -1);
                    }
                }
            }

            return null;
        }

        private static bool IsValidForRoom(RoomMatrix grid, OffsetCoords rolled, int i, int j, int qx, int qy)
        {
            var testX = rolled.X + i * qx;
            var testY = rolled.Y + j * qy;

            if (testX < 0 || testX >= grid.Size)
            {
                return false;
            }

            if (testY < 0 || testY >= grid.Size)
            {
                return false;
            }

            var currentRoom = grid.GetRoom(testX, testY);
            var isValid = currentRoom == null;
            return isValid;
        }
    }
}
