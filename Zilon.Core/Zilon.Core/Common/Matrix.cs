﻿using System;

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
            Items = items ?? throw new ArgumentNullException(nameof(items));
            Width = width;
            Height = height;
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
    }
}
