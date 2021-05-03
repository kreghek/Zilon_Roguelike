using System;
using System.Linq;

namespace Zilon.Core.Common
{
    public static class HexHelper
    {
        public static CubeCoords ConvertToCube(int offsetX, int offsetY)
        {
            var x = offsetX - ((offsetY - (offsetY & 1)) / 2);
            var z = offsetY;
            var y = -x - z;

            return new CubeCoords(x, y, z);
        }

        public static CubeCoords ConvertToCube(OffsetCoords offsetCoords)
        {
            return ConvertToCube(offsetCoords.X, offsetCoords.Y);
        }

        public static OffsetCoords ConvertToOffset(CubeCoords cube)
        {
            var col = cube.X + ((cube.Z - (cube.Z & 1)) / 2);
            var row = cube.Z;
            return new OffsetCoords(col, row);
        }

        public static float[] ConvertToWorld(int offsetX, int offsetY)
        {
            var rowOffset = offsetY % 2 == 0 ? 0 : 0.5f;
            return new[]
            {
                offsetX + rowOffset,
                (offsetY * 3f) / 4
            };
        }

        public static float[] ConvertToWorld(OffsetCoords coords)
        {
            return ConvertToWorld(coords.X, coords.Y);
        }

        public static OffsetCoords ConvertWorldToOffset(int worldX, int worldY, int size)
        {
            var axialCoords = ConvertWorldToAxial(worldX, worldY, size);
            var offsetCoords = ConvertAxialToOffset(axialCoords);
            return offsetCoords;
        }

        /// <summary>
        /// Возвращает смещения диагоналей по часовой стрелке.
        /// </summary>
        /// <returns> Массив со смещениями. </returns>
        /// <remarks>
        /// Основано на статье https://www.redblobgames.com/grids/hexagons/.
        /// </remarks>
        public static CubeCoords[] GetDiagonalOffsetClockwise()
        {
            // Начинаем с верхнего левого.
            var offsets = new[]
            {
                new CubeCoords(-1, +2, -1), new CubeCoords(-2, +1, +1), new CubeCoords(-1, -1, +2),
                new CubeCoords(+1, -2, +1), new CubeCoords(+2, -1, -1), new CubeCoords(+1, +1, -2)
            };

            return offsets;
        }

        /// <summary>
        /// Возвращает соседние координаты указанной точки.
        /// </summary>
        /// <param name="baseCoords"> Опорная точка, для которой возвращаются соседние координаты. </param>
        /// <returns> Набор соседних координат. </returns>
        public static CubeCoords[] GetNeighbors(CubeCoords baseCoords)
        {
            var offsets = GetOffsetClockwise();
            var neighborCoords = new CubeCoords[6];
            for (var i = 0; i < 6; i++)
            {
                var offset = offsets[i];
                neighborCoords[i] = offset + baseCoords;
            }

            return neighborCoords;
        }

        /// <summary>
        /// Возвращает соседние координаты указанной точки.
        /// </summary>
        /// <returns> Набор соседних координат. </returns>
        public static OffsetCoords[] GetNeighbors(int baseX, int baseY)
        {
            var baseCubeCoords = ConvertToCube(baseX, baseY);

            var neighborCoords = GetNeighbors(baseCubeCoords);

            return neighborCoords.Select(x => ConvertToOffset(x)).ToArray();
        }

        /// <summary>
        /// Возвращает смещения по часовой стрелке.
        /// </summary>
        /// <returns> Массив со смещениями. </returns>
        /// <remarks>
        /// Основано на статье https://www.redblobgames.com/grids/hexagons/.
        /// </remarks>
        public static CubeCoords[] GetOffsetClockwise()
        {
            var offsets = new[]
            {
                new CubeCoords(-1, +1, 0), new CubeCoords(-1, 0, +1), new CubeCoords(0, -1, +1),
                new CubeCoords(+1, -1, 0), new CubeCoords(+1, 0, -1), new CubeCoords(0, +1, -1)
            };

            return offsets;
        }

        private static OffsetCoords ConvertAxialToOffset(AxialCoords axialCoords)
        {
            static int round(float a)
            {
                return (int)Math.Round(a, MidpointRounding.ToEven);
            }

            var roundQ = round(axialCoords.Q);
            var roundR = round(axialCoords.R);

            var x = roundQ + (roundR / 2);
            var y = roundR;
            return new OffsetCoords(x, y);
        }

        private static AxialCoords ConvertWorldToAxial(int worldX, int worldY, int size)
        {
            // see https://habr.com/ru/post/319644/

            static float sqrt(float a)
            {
                return (float)Math.Sqrt(a);
            }

            var xDiv3 = worldX / 3f;
            var yDiv3 = worldY / 3f;
            var q = ((xDiv3 * sqrt(3)) - yDiv3) / size;
            var r = (yDiv3 * 2f) / size;
            var axialCoords = new AxialCoords(q, r);

            return axialCoords;
        }

        private struct AxialCoords : IEquatable<AxialCoords>
        {
            public AxialCoords(float q, float r)
            {
                Q = q;
                R = r;
            }

            public float Q { get; set; }
            public float R { get; set; }

            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = 1861411795;
                    hashCode = (hashCode * -1521134295) + Q.GetHashCode();
                    hashCode = (hashCode * -1521134295) + R.GetHashCode();
                    return hashCode;
                }
            }

            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
            public bool Equals(AxialCoords other)
            {
const float EPSILON = 0,000001f;
                return NearlyEqual(Q, other.Q,EPSILON ) && NearlyEqual( R, other.R, EPSILON) ;
            }

            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
            public override bool Equals(object obj)
            {
                return obj is AxialCoords coords && Equals(coords);
            }

            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
            public static bool operator ==(AxialCoords left, AxialCoords right)
            {
                return left.Equals(right);
            }

            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
            public static bool operator !=(AxialCoords left, AxialCoords right)
            {
                return !(left == right);
            }
private static boolean NearlyEqual(float a, float b, float epsilon) {
    final float absA = Math.abs(a);
    final float absB = Math.abs(b);
    final float diff = Math.abs(a - b);

    if (a == b) { // shortcut, handles infinities
        return true;
    } else if (a == 0 || b == 0 || absA + absB < Float.MIN_NORMAL) {
        // a or b is zero or both are extremely close to it
        // relative error is less meaningful here
        return diff < (epsilon * Float.MIN_NORMAL);
    } else { // use relative error
        return diff / (absA + absB) < epsilon;
    }
}
        }
    }
}
