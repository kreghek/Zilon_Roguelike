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
            var n = sourceMatrix.GetUpperBound(0);
            var m = sourceMatrix.GetUpperBound(1);
            T[,] ret = new T[m, n];

            for (int i = 0; i < m; ++i)
            {
                for (int j = 0; j < n; ++j)
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
    }
}
