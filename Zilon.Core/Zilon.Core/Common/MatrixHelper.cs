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
        public static T[,] RotateClockwise<T>(T[,] sourceMatrix)
        {
            if (sourceMatrix is null)
            {
                throw new System.ArgumentNullException(nameof(sourceMatrix));
            }

            var n = sourceMatrix.GetUpperBound(0) - sourceMatrix.GetLowerBound(0) + 1;
            var m = sourceMatrix.GetUpperBound(1) - sourceMatrix.GetLowerBound(1) + 1;
            T[,] ret = new T[m, n];

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
        public static T[,] Rotate<T>(T[,] sourceMatrix, MatrixRotation rotation)
        {
            var resultMatrix = sourceMatrix;
            for (var i = 0; i < (int)rotation; i++)
            {
                resultMatrix = RotateClockwise(resultMatrix);
            }

            return resultMatrix;
        }

        public static Matrix<bool> CreateScaledMatrix(Matrix<bool> matrix, int factor)
        {
            if (matrix is null)
            {
                throw new System.ArgumentNullException(nameof(matrix));
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
                            scaledMatrix.Items[i * factor + k1, j * factor + k2] = matrix.Items[i, j];
                        }
                    }
                }
            }

            return scaledMatrix;
        }
    }
}
