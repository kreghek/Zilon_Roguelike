using System;
using System.Collections.Generic;

namespace Zilon.Core.Common
{
    /// <summary>
    /// Вспомогательный класс для работы с кубическими координатами.
    /// </summary>
    public class CubeCoordHelper
    {
        private static float Lerp(int a, int b, float t)
        {
            return a + (b - a) * t;
        }

        private static CubeCoords LerpCube(CubeCoords a, CubeCoords b, float t)
        {
            return new CubeCoords((int)Math.Round(Lerp(a.X, b.X, t)),
                (int)Math.Round(Lerp(a.Y, b.Y, t)),
                (int)Math.Round(Lerp(a.Z, b.Z, t)));
        }

        /// <summary>
        /// Рисует линию в кубических координатах.
        /// </summary>
        /// <param name="a"> Начало линии. </param>
        /// <param name="b"> Конец линии. </param>
        /// <returns> Набор координат, составляющих линию. </returns>
        //TODO Добавить тест, возможно решащий проблему с непроходиыми комнатами.
        public static CubeCoords[] CubeDrawLine(CubeCoords a, CubeCoords b)
        {
            var n = a.DistanceTo(b);

            var list = new List<CubeCoords>();

            for (var i = 0; i < n; i++)
            {
                var point = LerpCube(a, b, 1.0f / n * i);
                list.Add(point);
            }

            return list.ToArray();
        }
    }
}
