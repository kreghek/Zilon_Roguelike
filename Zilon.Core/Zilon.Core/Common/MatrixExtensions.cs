namespace Zilon.Core.Common
{
    /// <summary>
    /// Расширения класса Matrix{T}.
    /// </summary>
    public static class MatrixExtensions
    {
        /// <summary>
        /// Создаёт копию исходной матрицы матрицы с добавлением единичных отступов по вертикали.
        /// Отступы заполняются значениеми по умолчанию.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns> Возвращает новую матрицу с отступами. </returns>
        /// <remarks>
        /// Это нужно для того, чтобы вертикальные коридоры не выходили за границу.
        /// Была проблема при соединении двух клеток (0, 4) и (0, 2).
        /// Правый край представляет собой "елку". При построении линии, которая должна стать коридором,
        /// будет выход координаты Х в -1. Того, второй узел коридора будет иметь координаты (-1, 3).
        /// См. тест CubeDrawLine_DifferentPoints_LineIsSolid(0,4,0,2).
        /// </remarks>
        public static Matrix<T> CreateMatrixWithVerticalMargins<T>(this Matrix<T> matrix)
        {
            if (matrix is null)
            {
                throw new System.ArgumentNullException(nameof(matrix));
            }

            const int MARGIN = 1;

            var sumMargin = MARGIN * 2;
            var marginMatrix = new Matrix<T>(matrix.Width + sumMargin, matrix.Height);

            for (var x = 0; x < matrix.Width; x++)
            {
                for (var y = 0; y < matrix.Height; y++)
                {
                    marginMatrix.Items[x + MARGIN, y] = matrix.Items[x, y];
                }
            }

            return marginMatrix;
        }
    }
}
