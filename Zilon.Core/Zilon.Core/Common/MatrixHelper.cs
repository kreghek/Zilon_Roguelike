namespace Zilon.Core.Common
{
    /// <summary>
    /// Вспомогательный класс для работы с матрицами.
    /// </summary>
    public static class MatrixHelper
    {
        /// <summary>
        /// Поворот матрицы на 90 градусов по часовой.
        /// </summary>
        /// <typeparam name="T"> Тип элементов массива. </typeparam>
        /// <param name="sourceMatrix"> Исходная матрица. </param>
        /// <returns> Возращает повёрнутую матрицу. </returns>
        public static Matrix<T> RotateClockwise<T>(Matrix<T> sourceMatrix)
        {
            if (sourceMatrix is null)
            {
                throw new System.ArgumentNullException(nameof(sourceMatrix));
            }

            var n = sourceMatrix.GetUpperBound(0) - sourceMatrix.GetLowerBound(0) + 1;
            var m = sourceMatrix.GetUpperBound(1) - sourceMatrix.GetLowerBound(1) + 1;
#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional
            // Отключаем это предупреждение, потому что на выходе нужен такой массив.
            // Кроме того, матрица не может быть jagged.
            T[,] ret = new T[m, n];
#pragma warning restore CA1814 // Prefer jagged arrays over multidimensional

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    ret[i, j] = sourceMatrix[n - j - 1, i];
                }
            }

            return ret;
        }

        /// <summary>
        /// Поворот матрицы на указанный угол.
        /// </summary>
        /// <typeparam name="T"> Тип элементов массива. </typeparam>
        /// <param name="sourceMatrix"> Исходная матрица. </param>
        /// <param name="rotation"> Угол поворота. </param>
        /// <returns> Возращает повёрнутую матрицу. </returns>
        public static Matrix<T> Rotate<T>(Matrix<T> sourceMatrix, MatrixRotation rotation)
        {
            var resultMatrix = sourceMatrix;
            for (var i = 0; i < (int)rotation; i++)
            {
                resultMatrix = RotateClockwise(resultMatrix);
            }

            return resultMatrix;
        }

        /// <summary>
        /// Растягивает матрицу.
        /// </summary>
        /// <param name="matrix"> Исходная матрица. </param>
        /// <param name="factor"> Фактор растягивания. Указывает, сколько ячеек будет создано в замен исходного значения. </param>
        /// <returns> Возвращает новую растянутую матрицу. </returns>
        /// <remarks>
        /// <see cref="factor"/> должен быть больше или равен 1. Уменьшение матрица не реализовано.
        /// </remarks>
        public static Matrix<bool> CreateScaledMatrix(Matrix<bool> matrix, int factor)
        {
            if (matrix is null)
            {
                throw new System.ArgumentNullException(nameof(matrix));
            }

            if (factor < 1)
            {
                throw new System.ArgumentException("Значение должно быть 1 или больше.", nameof(factor));
            }

            var scaledMatrix = new Matrix<bool>(matrix.Width * factor, matrix.Height * factor);

            for (var i = 0; i < matrix.Width; i++)
            {
                for (var j = 0; j < matrix.Height; j++)
                {
                    for (var k1 = 0; k1 < factor; k1++)
                    {
                        for (var k2 = 0; k2 < factor; k2++)
                        {
                            var iFactor = i * factor;
                            var jFactor = j * factor;
                            scaledMatrix.Items[iFactor + k1, jFactor + k2] = matrix.Items[i, j];
                        }
                    }
                }
            }

            return scaledMatrix;
        }
    }
}
