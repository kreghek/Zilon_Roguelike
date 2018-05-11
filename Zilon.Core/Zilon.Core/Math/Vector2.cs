namespace Zilon.Core.Math
{
    public struct Vector2
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"(X: {X:F2}, Y: {Y:F2})";
        }
    }
}
