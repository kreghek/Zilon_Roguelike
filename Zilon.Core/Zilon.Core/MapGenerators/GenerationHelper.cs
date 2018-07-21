namespace Zilon.Core.Tactics.Generation
{
    public static class GenerationHelper
    {
        public static OffsetCoords GetFreeCell(Room[,] grid, OffsetCoords rolled)
        {
            for (var i = 0; i < grid.GetLength(0); i++)
            {
                for (var j = 0; j < grid.GetLength(1); j++)
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

        private static bool IsValidForRoom(Room[,] grid, OffsetCoords rolled, int i, int j, int qx, int qy)
        {
            var testX = rolled.X + i * qx;
            var testY = rolled.Y + j * qy;

            if (testX < 0 || testX >= grid.GetLength(0))
            {
                return false;
            }

            if (testY < 0 || testY >= grid.GetLength(1))
            {
                return false;
            }

            var currentRoom = grid[testX, testY];
            var isValid = currentRoom == null;
            return isValid;
        }
    }
}
