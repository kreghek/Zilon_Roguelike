namespace Zilon.Core.Math
{
    public struct Vector2
    {
        public float X { get; set; }
        public float Y { get; set; }

        public override string ToString()
        {
            return $"(X: {X:D2}, Y: {Y:D2}";
        }
    }
}
