using System;

namespace Zilon.Core.Common
{
    public sealed class Matrix<T>
    {
        public Matrix(T[,] items, int width, int height)
        {
            Items = items ?? throw new ArgumentNullException(nameof(items));
            Width = width;
            Height = height;
        }

        public T[,] Items { get; }

        public int Width { get; }

        public int Height { get; }
    }
}
