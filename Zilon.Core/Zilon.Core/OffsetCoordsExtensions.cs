using System;

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
            if (coords == null)
            {
                return false;
            }

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
    }
}
