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

        public override bool Equals(object obj)
        {
            if (!(obj is Vector3))
            {
                return false;
            }

            var vector = (Vector3)obj;
            return X == vector.X &&
                   Y == vector.Y &&
                   Z == vector.Z;
        }

        public override int GetHashCode()
        {
            var hashCode = -307843816;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            hashCode = hashCode * -1521134295 + Z.GetHashCode();
            return hashCode;
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
