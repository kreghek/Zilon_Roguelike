using System.Security.Cryptography.X509Certificates;

namespace Zilon.Core
{
    public static class OffsetCoordsExtensions
    {
        /// <summary>Производит сравнение компонент координат с указанным значениями.</summary>
        /// <param name="coords">Проверяемые координаты.</param>
        /// <param name="x">Проверяемая x-компонента.</param>
        /// <param name="y">Проверяемая y-компонента.</param>
        /// <returns> Возвращает true, если компоненты координат равны указанным значениям. Иначе, false. </returns>
        public static bool CompsEqual(this OffsetCoords coords, int x, int y)
        {
            if (coords.X != x)
            {
                return false;
            }

            if (coords.Y != y)
            {
                return false;
            }

            return true;
        }

        public static OffsetCoords Left(this OffsetCoords coords)
        {
            return new OffsetCoords(coords.X - 1, coords.Y);
        }

        public static OffsetCoords LeftUp(this OffsetCoords coords)
        {
            if (coords.OffsetIsRight())
                return new OffsetCoords(coords.X, coords.Y - 1);

            return new OffsetCoords(coords.X - 1, coords.Y - 1);
        }

        public static OffsetCoords LeftDown(this OffsetCoords coords)
        {
            if (coords.OffsetIsRight())
                return new OffsetCoords(coords.X, coords.Y + 1);

            return new OffsetCoords(coords.X - 1, coords.Y + 1);
        }

        public static OffsetCoords Right(this OffsetCoords coords)
        {
            return new OffsetCoords(coords.X + 1, coords.Y);
        }

        public static OffsetCoords RightUp(this OffsetCoords coords)
        {
            if (coords.OffsetIsRight())
                return new OffsetCoords(coords.X + 1, coords.Y - 1);

            return new OffsetCoords(coords.X, coords.Y - 1);
        }

        public static OffsetCoords RightDown(this OffsetCoords coords)
        {
            if (coords.OffsetIsRight())
                return new OffsetCoords(coords.X + 1, coords.Y + 1);

            return new OffsetCoords(coords.X, coords.Y + 1);
        }

        private static bool OffsetIsRight(this OffsetCoords coords)
        {
            return coords.Y % 2 != 0;
        }
    }
}