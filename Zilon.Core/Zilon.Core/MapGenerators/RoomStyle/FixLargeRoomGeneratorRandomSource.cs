namespace Zilon.Core.MapGenerators.RoomStyle
{
    public class FixLargeRoomGeneratorRandomSource : FixRoomGeneratorRandomSourceBase, IRoomGeneratorRandomSource
    {
        public FixLargeRoomGeneratorRandomSource()
        {
            // Все комнаты пересекаются через всё доступное пространство.
            // Каждая комната из ряда пересекается с зеркальной комнатой.
            // Т.е. левая верхняя с правой нижней.

            for (var y = 0; y < 2; y++)
            {
                for (var x = 0; x < 5; x++)
                {
                    var current = new OffsetCoords(x, y);
                    var mirror = new OffsetCoords(5 - x, 5 - y);

                    Connections.Add(new Tuple<OffsetCoords, OffsetCoords>(
                        current,
                        mirror)
                    );
                }
            }
        }

        /// <summary>
        /// Выбрасывает случайный набор уникальных координат в матрице комнат указаной длины.
        /// </summary>
        /// <param name="roomGridSize">Размер матрицы комнат.</param>
        /// <param name="roomCount">Количество комнат в секторе.</param>
        /// <returns>
        /// Возвращает массив координат из матрицы комнат.
        /// </returns>
        public override IEnumerable<OffsetCoords> RollRoomMatrixPositions(int roomGridSize, int roomCount)
        {
            var result = new List<OffsetCoords>(20);

            for (var y = 0; y < 2; y++)
            {
                for (var x = 0; x < 5; x++)
                {
                    var current = new OffsetCoords(x, y);
                    var mirror = new OffsetCoords(5 - x, 5 - y);

                    result.Add(current);
                    result.Add(mirror);
                }
            }

            return result;
        }

        /// <summary>
        /// Выбрасывает случайный размер комнаты.
        /// </summary>
        /// <param name="minSize">Минимальный размер комнаты.</param>
        /// <param name="maxSize">Максимальный размер комнаты.</param>
        /// <returns>
        /// Возвращает размер с произвольными шириной и высотой в диапазоне (minSize, maxSize).
        /// </returns>
        /// <remarks>
        /// Источник рандома возвращает случайный размер комнаты в указанном диапазоне.
        /// </remarks>
        protected override Size RollRoomSize(int minSize, int maxSize)
        {
            return new Size(maxSize, maxSize);
        }
    }
}