using System;

namespace Zilon.Core.Common
{
    /// <summary>
    /// Матрица значений.
    /// </summary>
    /// <typeparam name="T"> Тип значений матрицы. </typeparam>
    public sealed class Matrix<T>
    {
        /// <summary>
        /// Конструктор матрицы значений.
        /// </summary>
        /// <param name="items"> Двумерный массив, который будет лежать в оснвое матрицы. </param>
        /// <param name="width"> Ширина матрицы. Должна соответствовать входному массиву. </param>
        /// <param name="height"> Высота матрицы. Должна соответствовать входному массиву. </param>
        public Matrix(T[,] items, int width, int height)
        {
            CheckArguments(width, height);

            Items = items ?? throw new ArgumentNullException(nameof(items));

            Width = width;
            Height = height;
        }

        /// <summary>
        /// Конструктор матрицы значений.
        /// </summary>
        /// <param name="width"> Ширина матрицы. Должна соответствовать входному массиву. </param>
        /// <param name="height"> Высота матрицы. Должна соответствовать входному массиву. </param>
        public Matrix(int width, int height)
        {
            CheckArguments(width, height);
            Width = width;
            Height = height;

            Items = new T[width, height];
        }

        /// <summary>
        /// Элементы матрицы.
        /// </summary>
        public T[,] Items { get; }

        /// <summary>
        /// Ширина матрицы.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Высота матрицы.
        /// </summary>
        public int Height { get; }

        public T this[int x, int y]
        {
            get
            {
                return Items[x, y];
            }
            set
            {
                Items[x, y] = value;
            }
        }

        public bool IsIn(int x, int y)
        {
            if (x >= Width || y >= Height)
            {
                return false;
            }

            if (x < 0 || y < 0)
            {
                return false;
            }

            return true;
        }

        private static void CheckArguments(int width, int height)
        {
            if (width <= 0)
            {
                throw new ArgumentException("Ширина должна быть больше 0.", nameof(width));
            }

            if (height <= 0)
            {
                throw new ArgumentException("Высота должна быть больше 0.", nameof(height));
            }
        }
    }
}