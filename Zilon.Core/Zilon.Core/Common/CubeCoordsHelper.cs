using System;
using System.Collections.Generic;

namespace Zilon.Core.Common
{
    /// <summary>
    /// Вспомогательный класс для работы с кубическими координатами.
    /// </summary>
    /// <remarks>
    /// Алгоритмы реализованы на основе статей:
    /// https://habr.com/post/319644/
    /// https://www.redblobgames.com/grids/hexagons/
    /// </remarks>
    public static class CubeCoordsHelper
    {
        private static float Lerp(int a, int b, float t)
        {
            return a + (b - a) * t;
        }

        private static void LerpCube(CubeCoords a, CubeCoords b, float t, out float x, out float y, out float z)
        {
            x = Lerp(a.X, b.X, t);
            y = Lerp(a.Y, b.Y, t);
            z = Lerp(a.Z, b.Z, t);
        }

        private static CubeCoords RoundCube(float x, float y, float z)
        {
            var rx = Math.Round(x);
            var ry = Math.Round(y);
            var rz = Math.Round(z);

            var xDiff = Math.Abs(rx - x);
            var yDiff = Math.Abs(ry - y);
            var zDiff = Math.Abs(rz - z);

            if (xDiff > yDiff && xDiff > zDiff)
            {
                rx = -ry - rz;
            }
            else if (yDiff > zDiff)
            {
                ry = -rx - rz;
            }
            else
            {
                rz = -rx - ry;
            }

            return new CubeCoords((int)rx, (int)ry, (int)rz);
        }

        /// <summary>
        /// Рисует линию в кубических координатах.
        /// </summary>
        /// <param name="a"> Начало линии. </param>
        /// <param name="b"> Конец линии. </param>
        /// <returns> Набор координат, составляющих линию. </returns>
        public static CubeCoords[] CubeDrawLine(CubeCoords a, CubeCoords b)
        {
            var n = a.DistanceTo(b);

            var list = new List<CubeCoords>();

            for (var i = 0; i <= n; i++)
            {
                LerpCube(a, b, 1.0f / n * i, out float cubeX, out float cubeY, out float cubeZ);
                var point = RoundCube(cubeX, cubeY, cubeZ);
                list.Add(point);
            }

            return list.ToArray();
        }
    }
}
