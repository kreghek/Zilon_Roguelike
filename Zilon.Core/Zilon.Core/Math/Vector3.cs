namespace Zilon.Core.Math
{
    public struct Vector3
    {
        public float X { get; }
        public float Y { get; }
        public float Z { get; }

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override string ToString()
        {
            return $"(X: {X:F2}, Y: {Y:F2}, Z: {Z:F2})";
        }

        public static bool operator ==(Vector3 v, Vector3 v2)
        {
            return v.X == v2.X && v.Y == v2.Y && v.Z == v2.Z;
        }

        public static bool operator !=(Vector3 v, Vector3 v2)
        {
            return !(v == v2);
        }
    }
}
