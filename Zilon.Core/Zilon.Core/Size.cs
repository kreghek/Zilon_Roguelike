namespace Zilon.Core
{
    public sealed class Size
    {
        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int Height { get; }
        public int Width { get; }

        public override string ToString()
        {
            return $"({Width}, {Height})";
        }
    }
}